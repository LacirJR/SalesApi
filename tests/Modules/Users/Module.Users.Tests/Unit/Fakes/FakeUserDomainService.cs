using Bogus;
using Module.Users.Application.Interfaces.Services;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeUserDomainService : IUserDomainService
{
    private readonly HashSet<string> _existingEmails;

    public FakeUserDomainService()
    {
        var faker = new Faker();
        _existingEmails = new HashSet<string>
        {
            "existing@email.com",
            faker.Internet.Email(),
            faker.Internet.Email()
        };
    }

    public Task<bool> ValidateEmailIsUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(!_existingEmails.Contains(email));
    }
}