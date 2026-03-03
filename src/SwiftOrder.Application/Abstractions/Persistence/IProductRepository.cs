using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken ct);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct);
    Task<List<Product>> ListAsync(CancellationToken ct);
}