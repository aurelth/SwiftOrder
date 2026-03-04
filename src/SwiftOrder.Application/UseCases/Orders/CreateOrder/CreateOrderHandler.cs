using FluentValidation;
using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Application.DTOs.Orders;
using SwiftOrder.Domain.Entities;

namespace SwiftOrder.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateOrderRequest> _validator;

    public CreateOrderHandler(
        IOrderRepository orders,
        IUnitOfWork uow,
        IValidator<CreateOrderRequest> validator)
    {
        _orders = orders;
        _uow = uow;
        _validator = validator;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request.Request, ct);

        var order = new Order(request.Request.CustomerName);

        await _orders.AddAsync(order, ct);
        await _uow.SaveChangesAsync(ct);

        return order.Id;
    }
}