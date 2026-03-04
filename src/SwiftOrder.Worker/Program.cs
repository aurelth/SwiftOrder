using SwiftOrder.Worker;
using SwiftOrder.Infrastructure.DependencyInjection;
using SwiftOrder.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<WorkerRabbitOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<SwiftOrder.Worker.Consumers.OrderSubmittedConsumer>();

var host = builder.Build();
host.Run();
