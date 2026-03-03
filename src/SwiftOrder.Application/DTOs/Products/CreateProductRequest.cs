namespace SwiftOrder.Application.DTOs.Products;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    decimal Price,
    int StockQuantity
);