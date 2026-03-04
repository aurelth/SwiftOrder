using Microsoft.EntityFrameworkCore;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Domain.Entities;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Infrastructure.Persistence;

public sealed class OrderItemWriter : IOrderItemWriter
{
    private readonly AppDbContext _db;
    private readonly IProductRepository _products;

    public OrderItemWriter(AppDbContext db, IProductRepository products)
    {
        _db = db;
        _products = products;
    }

    public async Task AddItemAsync(Guid orderId, Guid productId, int quantity, CancellationToken ct)
    {
        var order = await _db.Orders.AsTracking().FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null)
            throw new DomainException("Order not found.");

        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
            throw new DomainException("Product not found.");

        var item = new OrderItem(order.Id, product.Id, product.Name, product.Price, quantity);

        await _db.OrderItems.AddAsync(item, ct);

        // Update total based on DB + new item
        var currentTotal = await _db.OrderItems
            .Where(i => i.OrderId == order.Id)
            .SumAsync(i => (decimal?)i.LineTotal, ct) ?? 0m;

        // simplest: set via reflection to avoid changing domain right now
        order.GetType().GetProperty(nameof(Order.TotalAmount))!
            .SetValue(order, currentTotal + item.LineTotal);

        await _db.SaveChangesAsync(ct);
    }
}