using FluentAssertions;
using ShoppingCart.Api.Services;
using ShoppingCart.API.DTOs;

namespace ShoppingCart.Test;

public class CartServiceTests
{
    private readonly CartService _service;

    public CartServiceTests()
    {
        _service = new CartService();
    }

    [Fact]
    public void AddItem_WithValidRequest_ShouldAddItemToCart()
    {
        // Arrange
        var request = new AddItemRequest
        {
            ProductName = "Apple",
            Price = 10m,
            Quantity = 2
        };

        // Act
        var result = _service.AddItem(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ProductName.Should().Be("Apple");
        result.Data.Price.Should().Be(10m);
        result.Data.Quantity.Should().Be(2);
        _service.GetItems().Should().HaveCount(1);
    }

    [Fact]
    public void AddItem_WithInvalidPrice_ShouldReturnFailure()
    {
        // Arrange
        var request = new AddItemRequest
        {
            ProductName = "Apple",
            Price = 0m,
            Quantity = 2
        };

        // Act
        var result = _service.AddItem(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("BAD_REQUEST");
        _service.GetItems().Should().BeEmpty();
    }

    [Fact]
    public void UpdateItem_WithValidData_ShouldModifyQuantity()
    {
        // Arrange
        var addResult = _service.AddItem(new AddItemRequest
        {
            ProductName = "Banana",
            Price = 5m,
            Quantity = 3
        });
        var itemId = addResult.Data!.Id;
        var updateRequest = new UpdateItemRequest { Quantity = 10 };

        // Act
        var result = _service.UpdateItem(itemId, updateRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Quantity.Should().Be(10);
        result.Data.Id.Should().Be(itemId);
    }

    [Fact]
    public void UpdateItem_WithNonExistentId_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateItemRequest { Quantity = 5 };

        // Act
        var result = _service.UpdateItem(nonExistentId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void RemoveItem_WithValidId_ShouldRemoveItem()
    {
        // Arrange
        var addResult = _service.AddItem(new AddItemRequest
        {
            ProductName = "Orange",
            Price = 2m,
            Quantity = 2
        });
        var itemId = addResult.Data!.Id;

        // Act
        var result = _service.RemoveItem(itemId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _service.GetItems().Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_WithNonExistentId_ShouldReturnFailure()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _service.RemoveItem(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void GetCartSummary_ShouldCalculateCorrectTotals()
    {
        // Arrange
        _service.AddItem(new AddItemRequest { ProductName = "Item1", Price = 10m, Quantity = 2 });
        _service.AddItem(new AddItemRequest { ProductName = "Item2", Price = 5m, Quantity = 3 });

        // Act
        var summary = _service.GetCartSummary();

        // Assert
        summary.UniqueItems.Should().Be(2);
        summary.TotalQuantity.Should().Be(5);
        summary.TotalAmount.Should().Be(15m);
    }

    [Fact]
    public void CompleteWorkflow_ShouldHandleAddUpdateRemoveCorrectly()
    {
        // Arrange
        var addRequest = new AddItemRequest { ProductName = "Workflow Item", Price = 100m, Quantity = 1 };

        // Act & Assert - Add
        var addResult = _service.AddItem(addRequest);
        addResult.IsSuccess.Should().BeTrue();
        _service.GetItems().Should().HaveCount(1);

        // Act & Assert - Update
        var updateResult = _service.UpdateItem(addResult.Data!.Id, new UpdateItemRequest { Quantity = 5 });
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Data!.Quantity.Should().Be(5);

        // Act & Assert - Remove
        var removeResult = _service.RemoveItem(addResult.Data.Id);
        removeResult.IsSuccess.Should().BeTrue();
        _service.GetItems().Should().BeEmpty();
    }
}