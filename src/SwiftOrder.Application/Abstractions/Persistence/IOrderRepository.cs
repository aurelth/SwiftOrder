using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Order>> ListAsync(CancellationToken ct);
}