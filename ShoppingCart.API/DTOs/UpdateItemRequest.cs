using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.API.DTOs;

public class UpdateItemRequest
{
    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
    public int Quantity { get; set; }
}
