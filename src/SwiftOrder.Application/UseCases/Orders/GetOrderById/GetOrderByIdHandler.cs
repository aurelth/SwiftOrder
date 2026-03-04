using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Application.DTOs.Orders;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Application.UseCases.Orders.GetOrderById;

public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto>
{
    private readonly IOrderRepository _orders;

    public GetOrderByIdHandler(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<OrderDetailsDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            throw new DomainException("Order not found.");

        var items = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.ProductId,
            i.ProductNameSnapshot,
            i.UnitPrice,
            i.Quantity,
            i.LineTotal
        )).ToList();

        return new OrderDetailsDto(
            order.Id,
            order.OrderNumber,
            order.CustomerName,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.SubmittedAt,
            order.ConfirmedAt,
            items
        );
    }
}