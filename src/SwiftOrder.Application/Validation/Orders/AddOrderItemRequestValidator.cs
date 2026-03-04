using FluentValidation;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.Validation.Orders;

public class AddOrderItemRequestValidator : AbstractValidator<AddOrderItemRequest>
{
    public AddOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}