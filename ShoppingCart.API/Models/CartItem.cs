namespace ShoppingCart.Api.Models;

/// <summary>
/// Represents an item in the shopping cart
/// </summary>
public class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Calculates the total price for this cart item
    /// </summary>
    public decimal TotalPrice => Price * Quantity;
}
