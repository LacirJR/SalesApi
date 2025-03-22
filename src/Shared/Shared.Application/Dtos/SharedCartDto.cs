namespace Shared.Application.Dtos;

public class SharedCartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<SharedCartItemDto> Products { get; set; } = new();
    public decimal TotalPrice { get; set; }
}