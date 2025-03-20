using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Mappings;
using Module.Users.Application.Queries.GetUserByIdQuery;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using NSubstitute;
using Shared.Application.Interfaces.Persistence;
using Shared.Domain.Common;
using Shared.Domain.Common.Enums;
using Xunit;

namespace Module.Users.Tests.Unit.Handlers;

public class GetUserByIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetUserByIdQuery> _validator;
    private readonly IMapper _mapper;
    private readonly GetUserByIdQueryHandler _handler;
    
    public GetUserByIdQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _validator = Substitute.For<IValidator<GetUserByIdQuery>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        
        _mapper = config.CreateMapper();
        _handler = new GetUserByIdQueryHandler(_userRepository, _validator,_mapper);
    }
    
    
    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        var query = new GetUserByIdQuery(Guid.NewGuid());

        var validationFailures = new ValidationResult(new[]
        {
            new ValidationFailure("Id", "Invalid User ID format.")
        });

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(validationFailures);

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_Should_Return_Failed_When_User_Not_Found()
    {
        var query = new GetUserByIdQuery(Guid.NewGuid());

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.DefaultError.Type, result.Error.Type);
    }
    
    [Fact]
    public async Task Handle_Should_Return_User_When_Found()
    {
        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 10, "12345", new Geolocation("12.34", "56.78")),
            "999888777", UserRole.Admin, UserStatus.Active);

        var query = new GetUserByIdQuery(user.Id);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(user));

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Email, result.Data.Email);
        Assert.Equal(user.Username, result.Data.Username);
    }
    
}