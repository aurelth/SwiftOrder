using Microsoft.EntityFrameworkCore;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Product product, CancellationToken ct)
    {
        return _db.Products.AddAsync(product, ct).AsTask();
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();
        return _db.Products.FirstOrDefaultAsync(p => p.Sku == normalizedSku, ct);
    }

    public Task<List<Product>> ListAsync(CancellationToken ct)
    {
        return _db.Products
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }
}