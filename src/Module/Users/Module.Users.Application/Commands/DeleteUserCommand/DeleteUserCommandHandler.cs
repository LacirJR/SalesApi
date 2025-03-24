using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Users.Application.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ServiceResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteUserCommand> _validator;
    private readonly IMapper _mapper;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUserUnitOfWork unitOfWork, IValidator<DeleteUserCommand> validator, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserResponseDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var userDeleted = await _userRepository.RemoveByIdAsync(request.Id, cancellationToken);

        if (userDeleted is null)
            return ServiceResult.Failed<UserResponseDto>(ServiceError.NotFound);

        await _unitOfWork.CommitAsync(cancellationToken);
        
        return ServiceResult.Success(_mapper.Map<UserResponseDto>(userDeleted));
    }
}