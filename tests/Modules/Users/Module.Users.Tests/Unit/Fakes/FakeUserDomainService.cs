using Module.Users.Application.Interfaces.Services;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeUserDomainService : IUserDomainService
{
    private readonly HashSet<string> _existingEmails = new() { "existing@email.com" };

    public Task<bool> ValidateEmailIsUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_existingEmails.Contains(email));
    }
}