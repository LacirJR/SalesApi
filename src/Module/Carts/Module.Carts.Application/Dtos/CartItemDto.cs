﻿namespace Module.Carts.Application.Dtos;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public double DiscountPercentage { get; set; }

    public decimal FinalPrice =>
        Math.Round((UnitPrice * Quantity) - ((UnitPrice * (decimal)(DiscountPercentage / 100)) * Quantity), 2,
            MidpointRounding.AwayFromZero);


    public CartItemDto()
    {
    }

    public CartItemDto(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}