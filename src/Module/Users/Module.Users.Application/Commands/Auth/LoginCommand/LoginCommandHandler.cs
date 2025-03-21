using MediatR;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces;
using Module.Users.Application.Interfaces.Authentication;
using Module.Users.Application.Interfaces.Persistence;
using Shared.Domain.Common;

namespace Module.Users.Application.Commands.Auth.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResult<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ServiceResult<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null || !user.VerifyPassword(request.Password))
            throw new UnauthorizedAccessException("Invalid login attempt.");
        
        return ServiceResult.Success(_jwtTokenGenerator.GenerateToken(user));
    }
}