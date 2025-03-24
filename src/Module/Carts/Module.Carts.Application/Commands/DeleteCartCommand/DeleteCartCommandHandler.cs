using FluentValidation;
using MediatR;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.DeleteCartCommand;

public class DeleteCartCommandHandler :  IRequestHandler<DeleteCartCommand, ServiceResult<DeleteCartDto>>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteCartCommand> _validator;

    public DeleteCartCommandHandler(ICartRepository cartRepository, ICartUnitOfWork unitOfWork, IValidator<DeleteCartCommand> validator)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ServiceResult<DeleteCartDto>> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);

        if (cart is null)
            return ServiceResult.Failed<DeleteCartDto>(ServiceError.NotFound);

        await _cartRepository.RemoveByIdAsync(cart.Id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(new DeleteCartDto("Successfully deleted cart"));
    }
}