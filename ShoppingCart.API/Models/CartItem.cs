using ShoppingCart.Api.Common;

namespace ShoppingCart.Api.Models;


public class CartItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ProductName { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

    private CartItem()
    {

    }

    public static Result<CartItem> Create(string productName, decimal price, int quantity)
    {
        if (price <= 0)
            return Result<CartItem>.Failure("Price must be greater than zero.");

        if (quantity <= 0)
            return Result<CartItem>.Failure("Quantity must be greater than zero.");

        return Result<CartItem>.Success(new CartItem
        {
            ProductName = productName,
            Price = price,
            Quantity = quantity
        });
    }

    public Result<bool> UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            return Result<bool>.Failure("Quantity must be greater than zero.");

        Quantity = quantity;
        return Result<bool>.Success(true);
    }

    public decimal TotalPrice => Price * Quantity;
}
