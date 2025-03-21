using AutoMapper;
using MediatR;
using Module.Products.Application.Dtos;
using Module.Products.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Products.Application.Queries.GetProductsQuery;

public class GetProductsQueryHandler : IRequestHandler<GetAllProductsQuery,  ServiceResult<PaginatedList<ProductResponseDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginatedList<ProductResponseDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productsPaginated = await _productRepository.GetAllAsync(request.Filter, request.OrderBy, request.Page,
            request.Size,
            cancellationToken);

        var productsDto = _mapper.Map<List<ProductResponseDto>>(productsPaginated.Data);

        return ServiceResult.Success(new PaginatedList<ProductResponseDto>(productsDto, productsPaginated.TotalCount,
            request.Page, request.Size));
    
    }
}