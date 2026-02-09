using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ShoppingCart.Api.Common;
using ShoppingCart.Api.Models;
using ShoppingCart.Api.Services;
using ShoppingCart.API.Controllers;
using ShoppingCart.API.DTOs;

namespace ShoppingCart.Test;

public class CartControllerTests
{
    private readonly ICartService _mockService;
    private readonly CartController _sut;

    public CartControllerTests()
    {
        _mockService = Substitute.For<ICartService>();
        _sut = new CartController(_mockService);
    }

    [Fact]
    public void GetItems_WhenCartIsEmpty_ShouldReturnOkWithEmptyResponse()
    {
        // Arrange
        _mockService.GetItems().Returns(new List<CartItem>());
        _mockService.GetCartSummary().Returns(new CartSummaryDto());

        // Act
        var result = _sut.GetItems();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<CartResponse>().Subject;
        response.HasItems.Should().BeFalse();
        response.Items.Should().BeEmpty();
        response.Message.Should().Be("No items in the cart");
    }

    [Fact]
    public void GetItems_WhenCartHasItems_ShouldReturnOkWithItems()
    {
        // Arrange
        var item = CartItem.Create("Apple", 10m, 2).Data!;
        var items = new List<CartItem> { item };
        _mockService.GetItems().Returns(items);
        _mockService.GetCartSummary().Returns(new CartSummaryDto { UniqueItems = 1, TotalQuantity = 2 });

        // Act
        var result = _sut.GetItems();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<CartResponse>().Subject;
        response.HasItems.Should().BeTrue();
        response.Items.Should().HaveCount(1);
        response.SummaryDto.UniqueItems.Should().Be(1);
    }

    [Fact]
    public void AddItem_WithValidRequest_ShouldReturnOkWithCreatedItem()
    {
        // Arrange
        var request = new AddItemRequest { ProductName = "Apple", Price = 5m, Quantity = 1 };
        var createdItem = CartItem.Create("Apple", 5m, 1).Data!;
        _mockService.AddItem(request).Returns(Result<CartItem>.Success(createdItem));

        // Act
        var result = _sut.AddItem(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeAssignableTo<CartItem>().Subject;
        returnedItem.ProductName.Should().Be("Apple");
        returnedItem.Price.Should().Be(5m);
    }

    [Fact]
    public void AddItem_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddItemRequest { ProductName = "Apple", Price = 0m, Quantity = 1 };
        _mockService.AddItem(request).Returns(Result<CartItem>.Failure("BAD_REQUEST", "Invalid price"));

        // Act
        var result = _sut.AddItem(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().NotBeNull();
    }

    [Fact]
    public void UpdateItem_WithValidData_ShouldReturnOkWithUpdatedItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var request = new UpdateItemRequest { Quantity = 5 };
        var updatedItem = CartItem.Create("Apple", 10m, 5).Data!;
        _mockService.UpdateItem(itemId, request).Returns(Result<CartItem>.Success(updatedItem));

        // Act
        var result = _sut.UpdateItem(itemId, request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeAssignableTo<CartItem>().Subject;
        returnedItem.Quantity.Should().Be(5);
    }

    [Fact]
    public void UpdateItem_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var request = new UpdateItemRequest { Quantity = 5 };
        _mockService.UpdateItem(itemId, request).Returns(Result<CartItem>.Failure("NOT_FOUND", "Item not found"));

        // Act
        var result = _sut.UpdateItem(itemId, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void RemoveItem_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockService.RemoveItem(itemId).Returns(Result<bool>.Success(true));

        // Act
        var result = _sut.RemoveItem(itemId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void RemoveItem_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockService.RemoveItem(itemId).Returns(Result<bool>.Failure("NOT_FOUND", "Item not found"));

        // Act
        var result = _sut.RemoveItem(itemId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}