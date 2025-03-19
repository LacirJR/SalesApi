using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Domain.Common.Enums;

namespace Module.Users.Application.Commands.UpdateUserCommand;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
        IValidator<UpdateUserCommand> validator, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserResponseDto>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return ServiceResult.Failed<UserResponseDto>(ServiceError.NotFound);

        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            return ServiceResult.Failed<UserResponseDto>(
                ServiceError.ModelStateError($"Invalid role: {request.Role}"));

        if (!Enum.TryParse<UserStatus>(request.Status, true, out var userStatus))
            return ServiceResult.Failed<UserResponseDto>(
                ServiceError.ModelStateError($"Invalid status: {request.Status}"));


        user.Update(request.Email, request.Username, request.Password,
            new Name(request.Name.Firstname, request.Name.Lastname),
            new Address(request.Address.City, request.Address.Street, request.Address.Number, request.Address.Zipcode,
                new Geolocation(request.Address.Geolocation.Lat, request.Address.Geolocation.Long)), request.Phone,
            userRole, userStatus);

        _userRepository.Update(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ServiceResult.Success(_mapper.Map<UserResponseDto>(user));
    }
}