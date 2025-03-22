namespace Module.Carts.Application.Dtos;

public record UpdateCartDto(Guid UserId, DateTime Date, List<CartItemDto> Products) ;