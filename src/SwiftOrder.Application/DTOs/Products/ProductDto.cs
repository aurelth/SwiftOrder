namespace SwiftOrder.Application.DTOs.Products;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAt
);