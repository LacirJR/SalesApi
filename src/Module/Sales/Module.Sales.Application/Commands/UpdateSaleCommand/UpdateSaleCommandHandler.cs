using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.UpdateSaleCommand;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, ServiceResult<SaleResponseDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IValidator<UpdateSaleCommand> _validator;
    private readonly ISaleUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSaleCommandHandler(ISaleRepository saleRepository, IValidator<UpdateSaleCommand> validator,
        ISaleUnitOfWork unitOfWork, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SaleResponseDto>> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.NotFound);

        if (sale.IsCanceled)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.GenericError("Validation", "Update Forbidden", "Cannot update a canceled sale."));

        sale.UpdateBranchAndDate(request.Branch, request.Date);

        _saleRepository.Update(sale);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<SaleResponseDto>(sale));
    }
}