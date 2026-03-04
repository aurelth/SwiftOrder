using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Application.UseCases.Orders.SubmitOrder;

public sealed class SubmitOrderHandler : IRequestHandler<SubmitOrderCommand>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public SubmitOrderHandler(IOrderRepository orders, IUnitOfWork uow)
    {
        _orders = orders;
        _uow = uow;
    }

    public async Task Handle(SubmitOrderCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            throw new DomainException("Order not found.");

        order.Submit();

        await _uow.SaveChangesAsync(ct);
    }
}