using Microsoft.EntityFrameworkCore;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Order order, CancellationToken ct)
    {
        return _db.Orders.AddAsync(order, ct).AsTask();
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public Task<List<Order>> ListAsync(CancellationToken ct)
    {
        return _db.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }
}