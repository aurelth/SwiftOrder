using MediatR;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.UseCases.Orders.CreateOrder;

public sealed record CreateOrderCommand(CreateOrderRequest Request) : IRequest<Guid>;