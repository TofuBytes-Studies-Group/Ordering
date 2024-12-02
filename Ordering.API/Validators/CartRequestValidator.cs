using FluentValidation;

using Ordering.API.RequestDTOs;

namespace Ordering.API.Validators;

public class CartRequestValidator : AbstractValidator<CartDto>
{
    public CartRequestValidator()
    {
        RuleFor(x => x.CustomerUserName).NotEmpty().NotNull().WithMessage("CustomerUserName is required");
        RuleFor(x => x.CartItems).NotNull().NotEmpty().WithMessage("Cart items has to be provided");
        RuleForEach(x => x.CartItems).SetValidator(new CartItemRequestValidator());
    }
}