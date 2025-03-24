namespace Module.Sales.Application.Dtos;

public class SaleResponseDto
{
    public Guid Id { get; set; }
    public long Number { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public string Branch { get; set; }
    public Guid CartId { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsFinalized { get; set; }
    public decimal TotalValue { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}