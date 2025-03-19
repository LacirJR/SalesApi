﻿using FluentValidation;
using FluentValidation.Results;
using Module.Users.Application.Commands.CreateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Domain.Entities;
using Module.Users.Tests.Unit.Fakes;
using NSubstitute;
using Shared.Application.Interfaces.Persistence;
using Xunit;

namespace Module.Users.Tests.Unit.Handlers;

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<CreateUserCommand>>();

        _handler = new CreateUserCommandHandler(_userRepository, _unitOfWork, _validator);
    }

    [Fact]
    public async Task Handle_Should_Return_Failed_When_Validation_Fails()
    {
        var command = new CreateUserCommand(
            "invalid-email", "testuser", "123",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 1, "123456", new GeolocationDto(0, 0)),
            "999888777", "Active", "Admin"
        );

        var validationFailures = new ValidationResult(new[]
        {
            new ValidationFailure("Email", "Invalid email format.")
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationFailures);

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Return_Failed_When_Role_Is_Invalid()
    {
        var command = new CreateUserCommand(
            "test@example.com", "testuser", "123",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 1, "123456", new GeolocationDto(0, 0)),
            "999888777", "Active", "InvalidRole"
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("ModelStateError", result.Error.Type);
        Assert.Equal("Invalid role: InvalidRole", result.Error.Detail);
    }

    [Fact]
    public async Task Handle_Should_Return_Failed_When_Status_Is_Invalid()
    {
        var command = new CreateUserCommand(
            "test@example.com", "testuser", "admin123",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 1, "123456", new GeolocationDto(0, 0)),
            "999888777", "InvalidStatus", "Admin"
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("ModelStateError", result.Error.Type);
        Assert.Equal("Invalid status: InvalidStatus", result.Error.Detail);
    }

    [Fact]
    public async Task Handle_Should_Create_User_When_Valid()
    {
        var command = new CreateUserCommand(
            "test@example.com", "testuser", "admin123",
            new NameDto("Test", "User"),
            new AddressDto("City", "Street", 1, "123456", new GeolocationDto(0, 0)),
            "999888777", "Active", "Admin"
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotEqual(Guid.Empty, result.Data);
    }
}
