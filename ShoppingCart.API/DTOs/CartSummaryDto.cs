namespace ShoppingCart.API.DTOs;

public class CartSummaryDto
{
    public int UniqueItems { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}
