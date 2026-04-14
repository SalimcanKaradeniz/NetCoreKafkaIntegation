using Confluent.Kafka;
using KafkaDemo.Contracts.Models;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        Acks = Acks.All,    // En güvenli mod - tüm replica'lar onaylasın
        EnableIdempotence = true,   // Aynı mesajı iki kez yazma
    };
    return new ProducerBuilder<string, string>(config).Build();
});

var app = builder.Build();

app.MapPost("/orders", async (CreateOrderRequest request, IProducer<string, string> producer) =>
{
    var orderEvent = new OrderCreatedEvent
    (
        OrderId: Guid.NewGuid(),
        CustomerId: request.CustomerId,
        Amount: request.Amount,
        CreatedAt: DateTime.UtcNow
    );

    var message = new Message<string, string>
    {
        Key = orderEvent.CustomerId, // Aynı müşterinin siparişleri hep aynı partition'a gider
        Value = JsonSerializer.Serialize(orderEvent)
    };

    var result = await producer.ProduceAsync("orders-topic", message);

    return Results.Ok(new
    {
        OrderId = orderEvent.OrderId,
        Partition = result.Partition.Value,
        Offset = result.Offset.Value
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

record CreateOrderRequest(string CustomerId, decimal Amount);