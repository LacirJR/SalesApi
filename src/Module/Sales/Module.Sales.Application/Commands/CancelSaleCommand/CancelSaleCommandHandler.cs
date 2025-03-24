using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.CancelSaleCommand;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, ServiceResult<SaleResponseDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleUnitOfWork _unitOfWork;
    private readonly IValidator<CancelSaleCommand> _validator;
    private readonly IMapper _mapper;

    public CancelSaleCommandHandler(ISaleRepository saleRepository, ISaleUnitOfWork unitOfWork,
        IValidator<CancelSaleCommand> validator, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<SaleResponseDto>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.NotFound);

        if (sale.IsCanceled || sale.IsFinalized)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.GenericError("Validation", "Invalid Operation", "Sale is already finalized or canceled."));

        sale.Cancel();
        _saleRepository.Update(sale);

        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<SaleResponseDto>(sale));
    }
}
