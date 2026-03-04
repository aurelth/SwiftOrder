namespace SwiftOrder.Application.Abstractions.Persistence;

public interface IOrderItemWriter
{
    Task AddItemAsync(Guid orderId, Guid productId, int quantity, CancellationToken ct);
}