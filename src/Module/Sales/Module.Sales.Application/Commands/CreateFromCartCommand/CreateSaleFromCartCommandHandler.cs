using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Sales.Application.Dtos;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Entities;
using Module.Sales.Domain.Events;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.CreateFromCartCommand;

public class CreateSaleFromCartCommandHandler : IRequestHandler<CreateSaleFromCartCommand, ServiceResult<SaleResponseDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISharedCartService _sharedCartService;
    private readonly ISaleUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSaleFromCartCommand> _validator;

    public CreateSaleFromCartCommandHandler(
        ISaleRepository saleRepository,
        ISharedCartService sharedCartService,
        ISaleUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSaleFromCartCommand> validator)
    {
        _saleRepository = saleRepository;
        _sharedCartService = sharedCartService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<SaleResponseDto>> Handle(CreateSaleFromCartCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = await _sharedCartService.GetCartByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.NotFound);
        
        if(cart.IsFinalized)
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.GenericError("CartFinalized", "Cart is finalized","The cart has already been processed"));

        if (cart?.Products is null || !cart.Products.Any())
        {
            return ServiceResult.Failed<SaleResponseDto>(ServiceError.GenericError("Validation", "Empty cart", "The cart has no products."));
        }

        var sale = new Sale(cart.Date, cart.UserId, request.Branch, cart.Id);

        foreach (var item in cart.Products)
        {
            var saleItem = new SaleItem(sale.Id, item.ProductId, item.Quantity, item.UnitPrice, item.DiscountPercentage);
            sale.AddItem(saleItem);
        }
        
        await _saleRepository.AddAsync(sale, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        
        return ServiceResult.Success(_mapper.Map<SaleResponseDto>(sale));
    }
}