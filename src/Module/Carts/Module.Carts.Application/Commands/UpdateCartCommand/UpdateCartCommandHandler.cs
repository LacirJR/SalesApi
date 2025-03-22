using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.UpdateCartCommand;

public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, ServiceResult<CartResponseDto>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IDiscountRuleRepository _discountRuleRepository;
    private readonly ISharedProductService _sharedProductService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateCartCommand> _validator;

    public UpdateCartCommandHandler(ICartRepository cartRepository, IDiscountRuleRepository discountRuleRepository,
        ISharedProductService sharedProductService, IUnitOfWork unitOfWork, IMapper mapper,
        IValidator<UpdateCartCommand> validator)
    {
        _cartRepository = cartRepository;
        _discountRuleRepository = discountRuleRepository;
        _sharedProductService = sharedProductService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<CartResponseDto>> Handle(UpdateCartCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingCart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (existingCart is null)
            return ServiceResult.Failed<CartResponseDto>(ServiceError.NotFound);

        var rules = await _discountRuleRepository.GetActiveRulesAsync(cancellationToken);

        var products = new List<CartItem>();

        foreach (var item in request.Products)
        {
            var product = await _sharedProductService.GetProductByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                return ServiceResult.Failed<CartResponseDto>(ServiceError.GenericError("NotFound", "Resource not found",
                    $"Product {item.ProductId} not found."));
            
            products.Add(new CartItem(existingCart.Id, item.ProductId, item.Quantity, product.Price));
        }

        existingCart.UpdateItems(products);

        existingCart.ApplyRuleDiscount(rules);

        _cartRepository.Update(existingCart);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<CartResponseDto>(existingCart));
    }
}