using ShoppingCart.Api.Models;

namespace ShoppingCart.API.DTOs
{
    public class CartResponse
    {
        public bool HasItems { get; set; }
        public string Message { get; set; } = string.Empty;
        public IReadOnlyCollection<CartItem> Items { get; set; } = [];

        public CartSummaryDto SummaryDto { get; set; } = new();
    }
}
