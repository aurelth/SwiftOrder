using MediatR;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.UseCases.Orders.AddOrderItem;

public sealed record AddOrderItemCommand(Guid OrderId, AddOrderItemRequest Request) : IRequest;