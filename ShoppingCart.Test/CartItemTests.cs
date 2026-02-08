using FluentAssertions;
using ShoppingCart.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Test;

public class CartItemTests
{

    [Fact]
    public void Create_WithValidInput_ShouldReturnSuccess()
    {
        var result = CartItem.Create("Running Shoes", 200.5m, 2);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ProductName.Should().Be("Running Shoes");
        result.Data.Price.Should().Be(200.5m);
        result.Data.Quantity.Should().Be(2);
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldReturnFailure()
    {
        var result = CartItem.Create("Garmin watch", 0, 2);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("BAD_REQUEST");
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldSucceed()
    {
        var itemResult = CartItem.Create("Running shorts", 50, 3);
        var updateResult = itemResult.Data!.UpdateQuantity(10);

        updateResult.IsSuccess.Should().BeTrue();
        itemResult.Data.Quantity.Should().Be(10);
    }

    [Fact]
    public void TotalPrice_ShouldCalculateCorrectly()
    {
        var itemResult = CartItem.Create("Runnning arm bands", 2.5m, 4);

        itemResult.Data!.TotalPrice.Should().Be(10m); // 2.5 * 4
    }

}

