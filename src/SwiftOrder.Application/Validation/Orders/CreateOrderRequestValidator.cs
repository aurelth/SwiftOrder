using FluentValidation;
using SwiftOrder.Application.DTOs.Orders;

namespace SwiftOrder.Application.Validation.Orders;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200);
    }
}