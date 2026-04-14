using KafkaDemo.Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OrderConsumerService>();

var host = builder.Build();
host.Run();
