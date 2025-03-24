namespace Shared.Application.Dtos;

public class SharedCartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public bool IsFinalized { get; set; }
    public List<SharedCartItemDto> Products { get; set; } = new();
}