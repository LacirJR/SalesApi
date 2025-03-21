using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Commands.DeleteCategoryCommand;

public sealed record DeleteCategoryCommand(string CategoryName) : IRequest<ServiceResult<CategoryResponseDto>>;