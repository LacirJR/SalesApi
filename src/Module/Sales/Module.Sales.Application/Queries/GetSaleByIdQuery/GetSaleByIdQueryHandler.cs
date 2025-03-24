using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Sales.Application.Queries.GetSaleByIdQuery;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, ServiceResult<SaleResponseDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetSaleByIdQuery> _validator;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository, IMapper mapper, IValidator<GetSaleByIdQuery> validator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<SaleResponseDto>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);

        if (sale is null)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.NotFound);

        return ServiceResult.Success(_mapper.Map<SaleResponseDto>(sale));
    }
}
