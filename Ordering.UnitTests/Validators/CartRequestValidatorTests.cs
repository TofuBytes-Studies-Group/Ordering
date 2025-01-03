using FluentValidation.TestHelper;
using Ordering.API.RequestDTOs;
using Ordering.API.Validators;
using Xunit;
namespace Ordering.UnitTests.Validators;


public class CartRequestValidatorTests
{
    private readonly CartRequestValidator _validator = new CartRequestValidator();

    [Fact]
    public void Should_Have_Error_When_CustomerUsername_Is_Null()
    {
        var cartDto = new CartDto
        {
            CustomerUsername = null,
            CartItems = []
        };
        var result = _validator.TestValidate(cartDto);
        result.ShouldHaveValidationErrorFor(x => x.CustomerUsername);
    }

    [Fact]
    public void Should_Have_Error_When_CustomerUsername_Is_Empty()
    {
        var cartDto = new CartDto
        {
            CustomerUsername = string.Empty,
            CartItems = []
        };
        var result = _validator.TestValidate(cartDto);
        result.ShouldHaveValidationErrorFor(x => x.CustomerUsername);
    }

    [Fact]
    public void Should_Have_Error_When_CartItems_Is_Null()
    {
        var cartDto = new CartDto
        {
            CartItems = null,
            CustomerUsername = "imnotnull"
        };
        var result = _validator.TestValidate(cartDto);
        result.ShouldHaveValidationErrorFor(x => x.CartItems);
    }

    [Fact]
    public void Should_Have_Error_When_CartItems_Is_Empty()
    {
        var cartDto = new CartDto
        {
            CartItems = new List<CartItemDto>(),
            CustomerUsername = "imnotnull"
        };
        var result = _validator.TestValidate(cartDto);
        result.ShouldHaveValidationErrorFor(x => x.CartItems);
    }

    [Fact]
    public void Should_Not_Have_Error_When_CustomerUsername_And_CartItems_Are_Valid()
    {
        var cartDto = new CartDto
        {
            CustomerUsername = "john_doe",
            CartItems = new List<CartItemDto>
            {
                new CartItemDto { Dish = new DishDto(), Quantity = 1, SumPrice = 1 }
            }
        };
        var result = _validator.TestValidate(cartDto);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomerUsername);
        result.ShouldNotHaveValidationErrorFor(x => x.CartItems);
    }
}