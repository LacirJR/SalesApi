using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.DeleteProductCommand;

public record DeleteProductCommand(Guid Id) : IRequest<ServiceResult<ProductResponseDto>>;