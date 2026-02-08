namespace ShoppingCart.Api.Models;

public class AddItemRequest
{
    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
