using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Sales.Application.Queries.GetSaleByNumberQuery;

public class GetSaleByNumberQueryHandler : IRequestHandler<GetSaleByNumberQuery, ServiceResult<SaleResponseDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetSaleByNumberQuery> _validator;

    public GetSaleByNumberQueryHandler(ISaleRepository saleRepository, IMapper mapper, IValidator<GetSaleByNumberQuery> validator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<SaleResponseDto>> Handle(GetSaleByNumberQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByNumberAsync(request.Number, cancellationToken);

        if (sale is null)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.NotFound);

        return ServiceResult.Success(_mapper.Map<SaleResponseDto>(sale));
    }
}
