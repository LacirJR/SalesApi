namespace Module.Users.Application.Interfaces.Services;

public interface IUserDomainService
{
    Task<bool> ValidateEmailIsUniqueAsync(string email, CancellationToken cancellationToken);
}