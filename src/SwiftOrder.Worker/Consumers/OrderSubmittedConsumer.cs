using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SwiftOrder.Application.Abstractions.Messaging;
using SwiftOrder.Domain.Enums;
using SwiftOrder.Infrastructure.Persistence;

using RabbitConnection = RabbitMQ.Client.IConnection;
using RabbitChannel = RabbitMQ.Client.IModel;

namespace SwiftOrder.Worker.Consumers;

public sealed class OrderSubmittedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkerRabbitOptions _options;

    private RabbitConnection? _connection;
    private RabbitChannel? _channel;

    public OrderSubmittedConsumer(IServiceProvider serviceProvider, IOptions<WorkerRabbitOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true);

        _channel.QueueDeclare(
            queue: _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: _options.Queue,
            exchange: _options.Exchange,
            routingKey: "order.submitted"
        );

        // Avoid flooding this worker with too many unacked messages
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel not initialized.");

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var evt = JsonSerializer.Deserialize<OrderSubmittedEvent>(json);

                if (evt is null)
                {
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var order = db.Orders.AsTracking().FirstOrDefault(o => o.Id == evt.OrderId);
                if (order is null)
                {
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    return;
                }

                if (order.Status == OrderStatus.Submitted)
                {
                    order.MarkProcessing();
                    db.SaveChanges();

                    // simulate processing
                    Thread.Sleep(500);

                    order.Confirm();
                    db.SaveChanges();
                }

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch
            {
                // requeue message for retry
                _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: _options.Queue, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try { _channel?.Close(); } catch { /* ignore */ }
        try { _connection?.Close(); } catch { /* ignore */ }

        return base.StopAsync(cancellationToken);
    }
}