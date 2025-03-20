using AutoMapper;
using FluentValidation;
using MediatR;
using Module.Users.Application.Commands.UpdateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Users.Application.Queries.GetUserByIdQuery;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ServiceResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetUserByIdQuery> _validator;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IValidator<GetUserByIdQuery> validator, IMapper mapper)
    {
        _userRepository = userRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserResponseDto>> Handle(GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return ServiceResult.Failed<UserResponseDto>(ServiceError.DefaultError);

        return ServiceResult.Success(_mapper.Map<UserResponseDto>(user));
    }
}