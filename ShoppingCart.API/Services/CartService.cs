using ShoppingCart.Api.Common;
using ShoppingCart.Api.Models;
using ShoppingCart.API.DTOs;

namespace ShoppingCart.Api.Services;

public class CartService : ICartService
{
    private readonly List<CartItem> _items = new();

    public Result<CartItem> AddItem(AddItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            return Result<CartItem>.Failure("Quantity must be greater than zero.");
        }
        if (request.Price <= 0)
        {
            return Result<CartItem>.Failure("Price must be greater than zero.");
        }

        var item = new CartItem
        {
            Id = Guid.NewGuid(),
            ProductName = request.ProductName,
            Price = request.Price,
            Quantity = request.Quantity
        };

        _items.Add(item);

        return Result<CartItem>.Success(item);
    }

    public Result<CartItem> UpdateItem(Guid itemId, UpdateItemRequest request)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return Result<CartItem>.Failure("Item not found in cart.");
        }

        if (request.Quantity <= 0)
        {
            return Result<CartItem>.Failure("Quantity must be greater than zero.");
        }

        item.Quantity = request.Quantity;

        return Result<CartItem>.Success(item);
    }

    public Result<bool> RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return Result<bool>.Failure("Item not found in cart.");
        }

        _items.Remove(item);

        return Result<bool>.Success(true);
    }

    public IReadOnlyCollection<CartItem> GetItems()
    {
        return _items.AsReadOnly();
    }
}
