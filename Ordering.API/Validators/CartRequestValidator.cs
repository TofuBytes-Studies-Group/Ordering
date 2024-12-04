using FluentValidation;

using Ordering.API.RequestDTOs;

namespace Ordering.API.Validators;

public class CartRequestValidator : AbstractValidator<CartDto>
{
    public CartRequestValidator()
    {
        RuleFor(x => x.CustomerUsername).NotEmpty().NotNull().WithMessage("Customer Username is required");
        RuleFor(x => x.CartItems).NotNull().NotEmpty().WithMessage("Cart items has to be provided");
        RuleForEach(x => x.CartItems).SetValidator(new CartItemRequestValidator());
    }
}