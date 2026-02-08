using FluentAssertions;
using ShoppingCart.Api.Services;
using ShoppingCart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Test;
public class CartServiceTests
{
    private readonly CartService _service;

    public CartServiceTests()
    {
        _service = new CartService();
    }

    [Fact]
    public void AddItem_ValidRequest_ShouldReturnCartItem()
    {
        var request = new AddItemRequest { ProductName = "Apple", Price = 10, Quantity = 2 };
        var result = _service.AddItem(request);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ProductName.Should().Be("Apple");
        _service.GetItems().Count.Should().Be(1);
    }

    [Fact]
    public void AddItem_InvalidRequest_ShouldReturnFailure()
    {
        var request = new AddItemRequest { ProductName = "Apple", Price = 0, Quantity = 2 };
        var result = _service.AddItem(request);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("BAD_REQUEST");
        _service.GetItems().Should().BeEmpty();
    }

    [Fact]
    public void UpdateItem_Success_ShouldModifyQuantity()
    {
        var addResult = _service.AddItem(new AddItemRequest { ProductName = "Banana", Price = 5, Quantity = 3 });
        var updateResult = _service.UpdateItem(addResult.Data!.Id, new UpdateItemRequest { Quantity = 10 });

        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Data!.Quantity.Should().Be(10);
    }

    [Fact]
    public void UpdateItem_NonExistent_ShouldReturnFailure()
    {
        var result = _service.UpdateItem(Guid.NewGuid(), new UpdateItemRequest { Quantity = 5 });

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void RemoveItem_Success_ShouldRemoveItem()
    {
        var addResult = _service.AddItem(new AddItemRequest { ProductName = "Orange", Price = 2, Quantity = 2 });
        var removeResult = _service.RemoveItem(addResult.Data!.Id);

        removeResult.IsSuccess.Should().BeTrue();
        _service.GetItems().Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NonExistent_ShouldReturnFailure()
    {
        var result = _service.RemoveItem(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void GetCartSummary_ShouldReturnCorrectTotals()
    {
        _service.AddItem(new AddItemRequest { ProductName = "Item1", Price = 10, Quantity = 2 });
        _service.AddItem(new AddItemRequest { ProductName = "Item2", Price = 5, Quantity = 3 });

        var summary = _service.GetCartSummary();

        summary.UniqueItems.Should().Be(2);
        summary.TotalQuantity.Should().Be(5);
        summary.TotalAmount.Should().Be(15); 
    }

    [Fact]
    public void CompleteWorkflow_ShouldAddUpdateRemoveSuccessfully()
    {
        var addResult = _service.AddItem(new AddItemRequest { ProductName = "ItemX", Price = 1, Quantity = 1 });
        var updateResult = _service.UpdateItem(addResult.Data!.Id, new UpdateItemRequest { Quantity = 5 });
        var removeResult = _service.RemoveItem(addResult.Data!.Id);

        addResult.IsSuccess.Should().BeTrue();
        updateResult.IsSuccess.Should().BeTrue();
        removeResult.IsSuccess.Should().BeTrue();
        _service.GetItems().Should().BeEmpty();
    }
}
