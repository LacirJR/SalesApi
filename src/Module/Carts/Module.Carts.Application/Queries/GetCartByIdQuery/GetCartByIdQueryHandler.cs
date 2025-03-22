using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Carts.Application.Commands.DeleteCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Carts.Application.Queries.GetCartByIdQuery;

public class GetCartByIdQueryHandler : IRequestHandler<GetCartByIdQuery, ServiceResult<CartResponseDto>>
{
    
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetCartByIdQuery> _validator;


    public GetCartByIdQueryHandler(ICartRepository cartRepository, IMapper mapper, IValidator<GetCartByIdQuery> validator)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<ServiceResult<CartResponseDto>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
            return ServiceResult.Failed<CartResponseDto>(ServiceError.NotFound);

        return ServiceResult.Success(_mapper.Map<CartResponseDto>(cart));
    }
}