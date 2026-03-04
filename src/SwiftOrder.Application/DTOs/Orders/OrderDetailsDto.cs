using SwiftOrder.Domain.Enums;

namespace SwiftOrder.Application.DTOs.Orders;

public sealed record OrderDetailsDto(
    Guid Id,
    string? OrderNumber,
    string CustomerName,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime? SubmittedAt,
    DateTime? ConfirmedAt,
    List<OrderItemDto> Items
);