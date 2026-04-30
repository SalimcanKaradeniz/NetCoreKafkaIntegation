using Confluent.Kafka;
using KafkaDemo.Contracts.Models;
using System.Text.Json;

namespace KafkaDemo.Consumer;

public class OrderConsumerService : BackgroundService
{
    private readonly ILogger<OrderConsumerService> _logger;
    private readonly IConsumer<string, string> _consumer;

    public OrderConsumerService(ILogger<OrderConsumerService> logger)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "order-processing-group",  // Consumer group adı
            AutoOffsetReset = AutoOffsetReset.Earliest,  // Baştan oku
            EnableAutoCommit = false             // Manuel commit - güvenli
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {

        _consumer.Subscribe("orders-topic");
        _logger.LogInformation("Sipariş Consumer Başladı...");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 1 saniye bekle mesaj varsa al
                var result = _consumer.Consume(TimeSpan.FromSeconds(1));

                if (result == null) continue;

                var order = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);
                _logger.LogInformation($"Sipariş İşleniyor: {order.OrderId} | Müşteri: {order.CustomerId} | Tutar: {order.Amount}");

                // İş mantığın buraya gelir:
                await ProcessOrderAsync(order);

                // Başarılıysa offset'i commit et
                _consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Mesaj okuma hatası");
            }
        }

        _consumer.Close();
    }

    private async Task ProcessOrderAsync(OrderCreatedEvent order)
    {
        // DB'ye kaydet, e-posta gönder, stok güncelle vb.
        await Task.Delay(100); // Simüle ediyoruz
        _logger.LogInformation($"Sipariş {order.OrderId} işlendi");
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
