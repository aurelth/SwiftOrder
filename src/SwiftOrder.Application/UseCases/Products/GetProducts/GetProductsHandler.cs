using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Application.DTOs.Products;

namespace SwiftOrder.Application.UseCases.Products.GetProducts;

public sealed class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _products;

    public GetProductsHandler(IProductRepository products)
    {
        _products = products;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var list = await _products.ListAsync(ct);

        return list.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Sku,
            p.Price,
            p.StockQuantity,
            p.IsActive,
            p.CreatedAt
        )).ToList();
    }
}