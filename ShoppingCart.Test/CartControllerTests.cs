using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ShoppingCart.Api.Common;
using ShoppingCart.Api.Models;
using ShoppingCart.Api.Services;
using ShoppingCart.API.Controllers;
using ShoppingCart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Test
{
    public class CartControllerTests
    {
        private readonly ICartService _mockService;
        private readonly CartController _sut;

        public CartControllerTests()
        {
            _mockService = Substitute.For<ICartService>();
            _sut = new CartController(_mockService);
        }

        private static CartItem CreateValidCartItem(string productName = "Apple", decimal amount = 10m, int quantity = 2)
        {
            return CartItem.Create(productName, amount, quantity).Data!;
        }

        [Fact]
        public void GetItems_EmptyCart_ShouldReturnHasItemsFalse()
        {
            _mockService.GetItems().Returns(new List<CartItem>());

            var result = _sut.GetItems() as OkObjectResult;

            result!.Value.Should().BeOfType<CartResponse>();
            var response = (CartResponse)result.Value!;
            response.HasItems.Should().BeFalse();
        }

        [Fact]
        public void GetItems_WithItems_ShouldReturnHasItemsTrue()
        {
            var items = new List<CartItem> { CreateValidCartItem()};
            _mockService.GetItems().Returns(items);
            _mockService.GetCartSummary().Returns(new CartSummaryDto());

            var result = _sut.GetItems() as OkObjectResult;

            result!.Value.Should().BeOfType<CartResponse>();
            var response = (CartResponse)result.Value!;
            response.HasItems.Should().BeTrue();
            response.Items.Should().HaveCount(1);
        }

        [Fact]
        public void AddItem_ValidRequest_ShouldReturnOk()
        {
            var request = new AddItemRequest { ProductName = "Apple", Price = 5, Quantity = 1 };
            _mockService.AddItem(request).Returns(Result<CartItem>.Success(CreateValidCartItem()));

            var result = _sut.AddItem(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<CartItem>();
        }

        [Fact]
        public void AddItem_InvalidRequest_ShouldReturnBadRequest()
        {
            var request = new AddItemRequest { ProductName = "Apple", Price = 0, Quantity = 1 };
            _mockService.AddItem(request).Returns(Result<CartItem>.Failure("Invalid", "BAD_REQUEST"));

            var result = _sut.AddItem(request) as BadRequestObjectResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateItem_Success_ShouldReturnOk()
        {
            var id = Guid.NewGuid();
            var request = new UpdateItemRequest { Quantity = 5 };
            _mockService.UpdateItem(id, request).Returns(Result<CartItem>.Success(CreateValidCartItem()));

            var result = _sut.UpdateItem(id, request) as OkObjectResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void UpdateItem_NotFound_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();
            var request = new UpdateItemRequest { Quantity = 5 };
            _mockService.UpdateItem(id, request).Returns(Result<CartItem>.Failure("Not Found", "NOT_FOUND"));

            var result = _sut.UpdateItem(id, request) as NotFoundObjectResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteItem_Success_ShouldReturnNoContent()
        {
            var id = Guid.NewGuid();
            _mockService.RemoveItem(id).Returns(Result<bool>.Success(true));

            var result = _sut.RemoveItem(id) as NoContentResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void DeleteItem_NotFound_ShouldReturnNotFound()
        {
            var id = Guid.NewGuid();
            _mockService.RemoveItem(id).Returns(Result<bool>.Failure("Not found", "NOT_FOUND"));

            var result = _sut.RemoveItem(id) as NotFoundObjectResult;

            result.Should().NotBeNull();
        }
    }
}
