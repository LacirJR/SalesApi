using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Module.Users.Application.Commands.DeleteUserCommand;
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

public class DeleteUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteUserCommand> _validator;
    private readonly IMapper _mapper;
    private readonly DeleteUserCommandHandler _handler;
    
    public DeleteUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _validator = Substitute.For<IValidator<DeleteUserCommand>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        _mapper = config.CreateMapper();
        _handler = new DeleteUserCommandHandler(_userRepository, _unitOfWork, _validator, _mapper);
    }
    
    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        //var command = new DeleteUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"));
        var command = new DeleteUserCommand(Guid.Empty);

        var validationFailures = new ValidationResult(new[]
        {
            new ValidationFailure("Id", "Id cannot be empty")
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(validationFailures);

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
        
    [Fact]
    public async Task Handle_Should_Throw_Error_When_User_Not_Found()
    {
        var command = new DeleteUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"));
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.RemoveByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User?)null);
        
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
    }
    
    [Fact]
    public async Task Handle_Should_Delete_User_When_User_Exists()
    {
        var command = new DeleteUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"));
        
        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 10, "12345", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin,  UserStatus.Active);

        user.Id = Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05");
        
        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.RemoveByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);

        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
    }
    
}