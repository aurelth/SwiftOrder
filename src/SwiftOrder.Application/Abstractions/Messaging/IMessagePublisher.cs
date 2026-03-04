namespace SwiftOrder.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string routingKey, T message, CancellationToken ct);
}