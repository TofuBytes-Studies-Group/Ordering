using FluentValidation;
using Ordering.API.RequestDTOs;

namespace Ordering.API.Validators;

public class CartItemRequestValidator : AbstractValidator<CartItemDto>
{
    public CartItemRequestValidator()
    {
        RuleFor(x => x.Dish).NotNull().WithMessage("Dish cannot be null");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.SumPrice).GreaterThan(0).WithMessage("The summrized price must be greater than 0");
    }
}