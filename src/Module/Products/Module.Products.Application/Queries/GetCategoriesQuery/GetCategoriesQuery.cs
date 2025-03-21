using MediatR;
using Shared.Domain.Common;

namespace Module.Products.Application.Queries.GetCategoriesQuery;

public sealed record GetCategoriesQuery : IRequest<ServiceResult<List<string>>>;