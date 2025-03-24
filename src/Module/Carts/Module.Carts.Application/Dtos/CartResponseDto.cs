namespace Module.Carts.Application.Dtos;

public class CartResponseDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public bool IsFinalized { get; set; }
    public List<CartItemDto> Products { get; set; } = new();
    public decimal TotalPrice { get; set; }
}