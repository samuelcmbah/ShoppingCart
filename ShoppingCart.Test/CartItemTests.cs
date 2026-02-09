using FluentAssertions;
using ShoppingCart.Api.Models;

namespace ShoppingCart.Test;

public class CartItemTests
{
    [Fact]
    public void Create_WithValidInput_ShouldReturnSuccessResult()
    {
        // Arrange
        var productName = "Running Shoes";
        var price = 200.5m;
        var quantity = 2;

        // Act
        var result = CartItem.Create(productName, price, quantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ProductName.Should().Be(productName);
        result.Data.Price.Should().Be(price);
        result.Data.Quantity.Should().Be(quantity);
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithInvalidPrice_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = CartItem.Create("Garmin Watch", 0m, 2);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("BAD_REQUEST");
        result.Error.Message.Should().Contain("Price");
        result.Data.Should().BeNull();
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdate()
    {
        // Arrange
        var item = CartItem.Create("Running Shorts", 50m, 3).Data!;
        var newQuantity = 10;

        // Act
        var result = item.UpdateQuantity(newQuantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public void TotalPrice_ShouldCalculateCorrectly()
    {
        // Arrange
        var price = 2.5m;
        var quantity = 4;
        var item = CartItem.Create("Running Arm Bands", price, quantity).Data!;

        // Act
        var totalPrice = item.TotalPrice;

        // Assert
        totalPrice.Should().Be(10m);
    }
}