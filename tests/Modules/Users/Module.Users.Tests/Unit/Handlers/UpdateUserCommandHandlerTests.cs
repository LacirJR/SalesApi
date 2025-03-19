using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Module.Users.Application.Commands.UpdateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Mappings;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using NSubstitute;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common.Enums;
using Xunit;

namespace Module.Users.Tests.Unit.Handlers;

public class UpdateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly IMapper _mapper;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<UpdateUserCommand>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        _mapper = config.CreateMapper();
        _handler = new UpdateUserCommandHandler(_userRepository, _unitOfWork, _validator, _mapper);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"), "invalid-email", "testuser", "weakpassword",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 10, "12345", new GeolocationDto("12.34", "56.78")),
            "999888777", "Active", "Admin");

        var validationFailures = new ValidationResult(new[]
        {
            new ValidationFailure("Email", "Invalid email format.")
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationFailures);

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_Should_Return_Failed_When_User_Not_Found()
    {
        var command = new UpdateUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),
            "notfound@example.com", "testuser", "securePassword",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 10, "12345", new GeolocationDto("12.34", "56.78")),
            "999888777", "Active", "Admin"
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error.Type);
    }
    
    [Fact]
    public async Task Handle_Should_Return_Failed_When_Role_Is_Invalid()
    {
        var command = new UpdateUserCommand( Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),
            "test@example.com", "testuser", "securePassword",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 10, "12345", new GeolocationDto("12.34", "56.78")),
            "999888777", "Active", "InvalidRole"
        );

        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 10, "12345", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("ModelStateError", result.Error.Type);
        Assert.Equal("Invalid role: InvalidRole", result.Error.Detail);
    }
    
    [Fact]
    public async Task Handle_Should_Return_Failed_When_Status_Is_Invalid()
    {
        // Arrange
        var command = new UpdateUserCommand( Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),
            "test@example.com", "testuser", "securePassword",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 10, "12345", new GeolocationDto("12.34", "56.78")),
            "999888777", "InvalidStatus", "Admin"
        );

        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 10, "12345", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("ModelStateError", result.Error.Type);
        Assert.Equal("Invalid status: InvalidStatus", result.Error.Detail);
    }
    
    [Fact]
    public async Task Handle_Should_Update_User_When_Valid()
    {
        var command = new UpdateUserCommand( Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),
            "test@example.com", "testuser", "securePassword",
            new NameDto("Updated", "User"),
            new AddressDto("NewCity", "NewStreet", 100, "98765", new GeolocationDto("21.43", "65.87")),
            "111222333", "Active", "Admin"
        );

        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 10, "12345", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin,  UserStatus.Active);

        user.Id = Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05");
                
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    
}