using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Moto.Rentals.Api.Domain;

namespace Moto.Rentals.Api.Infra;

public sealed class RabbitConsumerHostedService(
    IConnection connection, IServiceProvider sp, IConfiguration cfg, ILogger<RabbitConsumerHostedService> log) : BackgroundService
{
    private IModel? _ch;
    private readonly string _exchange = cfg["Rabbit:Exchange"] ?? "Moto.motorcycles";
    private readonly string _queue = cfg["Rabbit:Queue"] ?? "Moto.motorcycles.created";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ch = connection.CreateModel();
        _ch.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
        _ch.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind(_queue, _exchange, "motorcycle.created");

        _ch.BasicQos(0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_ch);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msg = JsonSerializer.Deserialize<MotorcycleCreatedMessage>(json);

                if (msg is not null && msg.Year == 2024)
                {
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.YearNotifications.Add(new YearNotification
                    {
                        MotorcycleId = msg.Id,
                        Year = msg.Year
                    });
                    await db.SaveChangesAsync(stoppingToken);
                    log.LogInformation("Stored YearNotification for {MotorcycleId}", msg.Id);
                }

                _ch.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing message");
                _ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _ch.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _ch?.Dispose();
        base.Dispose();
    }
}

public sealed record MotorcycleCreatedMessage(Guid Id, string Identifier, int Year, string Model, string Plate);
