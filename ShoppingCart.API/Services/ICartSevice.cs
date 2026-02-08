using ShoppingCart.Api.Common;
using ShoppingCart.Api.Models;

namespace ShoppingCart.Api.Services;

public interface ICartService
{
    Result<CartItem> AddItem(AddItemRequest request);

    Result<CartItem> UpdateItem(Guid itemId, UpdateItemRequest request);

    Result<bool> RemoveItem(Guid itemId);

    IReadOnlyCollection<CartItem> GetItems();
}
