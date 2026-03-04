using FluentValidation;
using MediatR;
using SwiftOrder.Application.Abstractions.Persistence;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.UseCases.Orders.AddOrderItem;

public sealed class AddOrderItemHandler : IRequestHandler<AddOrderItemCommand>
{
    private readonly IOrderItemWriter _writer;
    private readonly IValidator<AddOrderItemRequest> _validator;

    public AddOrderItemHandler(IOrderItemWriter writer, IValidator<AddOrderItemRequest> validator)
    {
        _writer = writer;
        _validator = validator;
    }

    public async Task Handle(AddOrderItemCommand request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request.Request, ct);
        await _writer.AddItemAsync(request.OrderId, request.Request.ProductId, request.Request.Quantity, ct);
    }
}