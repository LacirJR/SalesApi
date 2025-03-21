namespace Module.Carts.Application.Dtos;

public class CartResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}