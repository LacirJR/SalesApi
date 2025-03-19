using System.ComponentModel.DataAnnotations;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Domain.Exceptions;

namespace Module.Users.Application.Services;

public class UserDomainService :  IUserDomainService
{
    private readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ValidateEmailIsUniqueAsync(string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        return existingUser is not null;
    }
}