using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.UpdateProductCommand;

public record UpdateProductCommand(Guid Id,string Title, decimal Price, string Description, string Category, string Image, RatingDto Rating) : IRequest<ServiceResult<ProductResponseDto>>;
