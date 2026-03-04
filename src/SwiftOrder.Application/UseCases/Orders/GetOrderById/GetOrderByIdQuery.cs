using MediatR;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.UseCases.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsDto>;