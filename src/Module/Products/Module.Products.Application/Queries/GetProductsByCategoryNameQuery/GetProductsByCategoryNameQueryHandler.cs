using AutoMapper;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Products.Application.Queries.GetProductsByCategoryNameQuery;

public class GetProductsByCategoryNameQueryHandler : IRequestHandler<GetProductsByCategoryNameQuery,
    ServiceResult<PaginatedList<ProductResponseDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductsByCategoryNameQueryHandler(IProductRepository productRepository, IMapper mapper,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }

    public async Task<ServiceResult<PaginatedList<ProductResponseDto>>> Handle(GetProductsByCategoryNameQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByNameAsync(request.CategoryName, cancellationToken);

        if (category is null)
            return ServiceResult.Failed<PaginatedList<ProductResponseDto>>(ServiceError.GenericError("NotFound",
                "Resource not found", "Category not found."));

        var productsPaginated = await _productRepository.GetAllAsync($"categoryId={category.Id}", request.Order, request.Page,
            request.Size, cancellationToken);
        
        var productsDto = _mapper.Map<List<ProductResponseDto>>(productsPaginated.Data);

        return ServiceResult.Success(new PaginatedList<ProductResponseDto>(productsDto, productsPaginated.TotalItems,
            request.Page, request.Size));
    }
}