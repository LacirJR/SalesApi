using Module.Users.Application.Commands.Auth.LoginCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Module.Users.Tests.Unit.Handlers;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

        _handler = new LoginCommandHandler(_userRepository, _jwtTokenGenerator);
    }
    
    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedAccessException_When_User_Not_Found()
    {
        var command = new LoginCommand("nonexistent@example.com", "password123");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedAccessException_When_Password_Is_Incorrect()
    {
        var user = FakeUser.CreateFakeUser();

        var command = new LoginCommand("valid@example.com", "wrongpassword");

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_Should_Return_Token_When_Login_Is_Successful()
    {
        var user = FakeUser.CreateFakeUser();

        var command = new LoginCommand("valid@example.com", "correctpassword");

        var token = new LoginResponseDto("mocked-jwt-token", null);

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _jwtTokenGenerator.GenerateToken(user).Returns(token);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal("mocked-jwt-token", result.Data.Token);
    }
}