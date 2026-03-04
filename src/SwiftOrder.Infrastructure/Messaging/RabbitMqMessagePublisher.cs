using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SwiftOrder.Application.Abstractions.Messaging;

namespace SwiftOrder.Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisher : IMessagePublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitMqMessagePublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: routingKey,
            basicProperties: props,
            body: body
        );

        return Task.CompletedTask;
    }
}