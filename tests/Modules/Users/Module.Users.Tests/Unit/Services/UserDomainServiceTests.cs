using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Services;
using Module.Users.Domain.Entities;
using Module.Users.Domain.ValueObjects;
using NSubstitute;
using Shared.Domain.Common.Enums;
using Xunit;

namespace Module.Users.Tests.Unit.Services;

public class UserDomainServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly UserDomainService _userDomainService;

    public UserDomainServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userDomainService = new UserDomainService(_userRepository);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailAlreadyExists()
    {
        var email = "test@example.com";
        _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(new User(email, "username", "password", new Name("John", "Doe"), null!, "123456789",
                UserRole.Customer));

        var result = await _userDomainService.ValidateEmailIsUniqueAsync(email, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        var email = "notfound@example.com";
        _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _userDomainService.ValidateEmailIsUniqueAsync(email, CancellationToken.None);

        Assert.False(result);
    }
}