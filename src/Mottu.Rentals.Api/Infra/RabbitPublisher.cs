using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Moto.Rentals.Api.Infra;

public sealed class RabbitPublisher(IConnection connection, IConfiguration cfg)
{
    private readonly string _exchange = cfg["Rabbit:Exchange"] ?? "Moto.motorcycles";
    public void Publish(string routingKey, object payload)
    {
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        var props = channel.CreateBasicProperties();
        props.ContentType = "application/json";
        channel.BasicPublish(_exchange, routingKey, props, body);
    }
}
