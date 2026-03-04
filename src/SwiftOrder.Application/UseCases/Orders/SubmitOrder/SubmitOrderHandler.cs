using MediatR;
using SwiftOrder.Application.Abstractions.Messaging;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Application.UseCases.Orders.SubmitOrder;

public sealed class SubmitOrderHandler : IRequestHandler<SubmitOrderCommand>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IMessagePublisher _publisher;

    public SubmitOrderHandler(IOrderRepository orders, IUnitOfWork uow, IMessagePublisher publisher)
    {
        _orders = orders;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task Handle(SubmitOrderCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            throw new DomainException("Order not found.");

        // Generate OrderNumber (unique enough for MVP)
        if (string.IsNullOrWhiteSpace(order.OrderNumber))
        {
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var suffix = order.Id.ToString("N")[..6].ToUpperInvariant();
            order.SetOrderNumber($"SO-{stamp}-{suffix}");
        }

        order.Submit();
        await _uow.SaveChangesAsync(ct);

        await _publisher.PublishAsync(
            routingKey: "order.submitted",
            message: new OrderSubmittedEvent(order.Id, order.OrderNumber!, order.SubmittedAt!.Value),
            ct: ct
        );
    }
}