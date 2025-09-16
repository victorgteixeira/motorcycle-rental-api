using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Moto.Rentals.Api.Infra;
using Moto.Rentals.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<FileStorageService>();

// RabbitMQ connection singleton (async consumers)
builder.Services.AddSingleton<IConnection>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory
    {
        HostName = cfg["Rabbit:Host"] ?? "localhost",
        UserName = cfg["Rabbit:User"] ?? "guest",
        Password = cfg["Rabbit:Pass"] ?? "guest",
        DispatchConsumersAsync = true,
        AutomaticRecoveryEnabled = true,         
        TopologyRecoveryEnabled = true,          
        RequestedHeartbeat = TimeSpan.FromSeconds(30),
        NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
    };
    return factory.CreateConnection();
});
builder.Services.AddSingleton<RabbitPublisher>();
builder.Services.AddHostedService<RabbitConsumerHostedService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
