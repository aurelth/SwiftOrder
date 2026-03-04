using MediatR;

namespace SwiftOrder.Application.UseCases.Orders.SubmitOrder;

public sealed record SubmitOrderCommand(Guid OrderId) : IRequest;