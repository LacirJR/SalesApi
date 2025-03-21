namespace Module.Products.Application.Dtos;

public sealed record UpdateProductDto(string Title, decimal Price, string Description, string Category, string Image, RatingDto Rating);