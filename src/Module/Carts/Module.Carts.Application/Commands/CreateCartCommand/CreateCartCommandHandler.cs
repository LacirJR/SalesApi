using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.CreateCartCommand;

public class CreateCartCommandHandler :  IRequestHandler<CreateCartCommand, ServiceResult<CartResponseDto>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IDiscountRuleRepository _discountRuleRepository;
    private readonly ISharedProductService _sharedProductService;
    private readonly ICartUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCartCommand> _validator;

    public CreateCartCommandHandler(ICartRepository cartRepository, ICartUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateCartCommand> validator, IDiscountRuleRepository discountRuleRepository, ISharedProductService sharedProductService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
        _discountRuleRepository = discountRuleRepository;
        _sharedProductService = sharedProductService;
    }

    public async Task<ServiceResult<CartResponseDto>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var existingCart = await _cartRepository.GetCartByUserIdAsync(request.UserId, cancellationToken);
        if (existingCart is not null)
        {
            return ServiceResult.Failed<CartResponseDto>(
                ServiceError.GenericError("ValidationError","ActiveCartExists", "An active cart already exists for this user."));
        }
        
        var newCart = new Cart(request.UserId, request.Date);
        var rules = await _discountRuleRepository.GetActiveRulesAsync(cancellationToken);
        
        foreach (var item in request.Products)
        {
            var product = await _sharedProductService.GetProductByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                return ServiceResult.Failed<CartResponseDto>(ServiceError.GenericError( "NotFound", "Resource Not Found", $"Product {item.ProductId} not found."));

            var cartItem = new CartItem(newCart.Id, item.ProductId, item.Quantity, product.Price);
            newCart.AddItem(cartItem);
        }
        
        newCart.ApplyRuleDiscount(rules);

        await _cartRepository.AddAsync(newCart, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return ServiceResult.Success(_mapper.Map<CartResponseDto>(newCart));
        
    }
}