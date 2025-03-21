using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.CreateProductCommand;

public record CreateProductCommand(string Title, decimal Price, string Description, string Category, string Image, RatingDto Rating) : IRequest<ServiceResult<ProductResponseDto>>;