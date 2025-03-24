using AutoMapper;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Sales.Application.Queries.GetAllSalesQuery;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, ServiceResult<PaginatedList<SaleResponseDto>>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetAllSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginatedList<SaleResponseDto>>> Handle(GetAllSalesQuery request,
        CancellationToken cancellationToken)
    {
        var salesPaginated = await _saleRepository.GetAllAsync(request.Filter, request.OrderBy, request.Page,
            request.Size, cancellationToken);
        var salesDto = _mapper.Map<List<SaleResponseDto>>(salesPaginated.Data);

        return ServiceResult.Success(new PaginatedList<SaleResponseDto>(
            salesDto, salesPaginated.TotalItems, request.Page, request.Size));
    }
}