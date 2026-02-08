using ShoppingCart.Api.Common;
using ShoppingCart.Api.Models;
using ShoppingCart.API.DTOs;

namespace ShoppingCart.Api.Services;

public class CartService : ICartService
{
    private readonly List<CartItem> _items = [];

    public Result<CartItem> AddItem(AddItemRequest request)
    {
        var itemResult = CartItem.Create(
            request.ProductName,
            request.Price,
            request.Quantity);

        if (!itemResult.IsSuccess)
        {
            return itemResult;
        }
        _items.Add(itemResult.Data!);

        return itemResult;
    }

    public Result<CartItem> UpdateItem(Guid itemId, UpdateItemRequest request)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return Result<CartItem>.Failure("NOT_FOUND", "Item not found in cart.");
        }

        var result = item.UpdateQuantity(request.Quantity);

        if (!result.IsSuccess)
        {
            return Result<CartItem>.Failure(
                result.Error!.Code,
                result.Error.Message
            );
        }

        return Result<CartItem>.Success(item);
    }

    public Result<bool> RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return Result<bool>.Failure("NOT_FOUND", "Item not found in cart.");
        }

        _items.Remove(item);

        return Result<bool>.Success(true);
    }

    public IReadOnlyCollection<CartItem> GetItems()
    {
        return _items.AsReadOnly();
    }

    public CartSummaryDto GetCartSummary()
    {
        return new CartSummaryDto
        {
            UniqueItems = _items.Count,
            TotalQuantity = _items.Sum(i => i.Quantity),
            TotalAmount = _items.Sum(i => i.Price)
        };
    }
}
