using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.CreateCategoryCommand;

public record CreateCategoryCommand(string CategoryName) : IRequest<ServiceResult<CategoryResponseDto>>;