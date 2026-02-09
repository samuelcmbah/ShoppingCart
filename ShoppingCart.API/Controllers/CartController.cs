using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Api.Common;
using ShoppingCart.Api.Services;
using ShoppingCart.API.DTOs;

namespace ShoppingCart.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetItems()
        {
            var items = _cartService.GetItems();
            var summary = _cartService.GetCartSummary();
            if (items.Count == 0)
            {
                return Ok(new CartResponse
                {
                    HasItems = false,
                    Message = "No items in the cart",
                    Items = [],
                    SummaryDto = summary

                });
            }

            return Ok(new CartResponse
            {
                HasItems = true,
                Message = "Cart items retrieved successfully",
                Items = items,
                SummaryDto = summary

            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddItem([FromBody] AddItemRequest request)
        {
            var result = _cartService.AddItem(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Data);
        }

        [HttpPut("{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateItem(Guid itemId, [FromBody] UpdateItemRequest request)
        {
            var result = _cartService.UpdateItem(itemId, request);

            if (!result.IsSuccess)
            {
                HandleError(result.Error!);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult RemoveItem(Guid itemId)
        {
            var result = _cartService.RemoveItem(itemId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return NoContent();
        }

        /// <summary>
        /// Maps error codes to appropriate HTTP responses
        /// </summary>
        private IActionResult HandleError(Error error)
        {
            return error.Code switch
            {
                "NOT_FOUND" => NotFound(error),
                "BAD_REQUEST" => BadRequest(error),
                _ => BadRequest(error) // Default fallback for unexpected errors
            };
        }
    }
}
