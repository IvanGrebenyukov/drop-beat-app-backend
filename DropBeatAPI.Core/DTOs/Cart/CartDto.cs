namespace DropBeatAPI.Core.DTOs.Cart;

public class CartDto
{
    public Guid CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}