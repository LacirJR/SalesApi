using MediatR;
using Module.Products.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Products.Application.Queries.GetCategoriesQuery;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ServiceResult<List<string>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<List<string>>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        if (!categories.Any())
            return ServiceResult.Success<List<string>>(new());

        return ServiceResult.Success(categories.Select(category => category.Name).ToList());
    }
}