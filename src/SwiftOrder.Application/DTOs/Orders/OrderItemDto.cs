namespace SwiftOrder.Application.DTOs.Orders;

public sealed record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductNameSnapshot,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);