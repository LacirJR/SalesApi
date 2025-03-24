using AutoMapper;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Persistence;
using Module.Users.Application.Mappings;
using Module.Users.Application.Queries.GetUsersQuery;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using NSubstitute;
using Shared.Domain.Common.Enums;
using Shared.Infrastructure.Common;
using Xunit;

namespace Module.Users.Tests.Unit.Handlers;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        _mapper = config.CreateMapper();

        _handler = new GetUsersQueryHandler(_userRepository, _mapper);
    }
    
    [Fact]
    public async Task Handle_Should_Return_EmptyList_When_No_Users_Exist()
    {
        var query = new GetUsersQuery();

        var emptyPaginatedList = new PaginatedList<User>(new List<User>(), 0, 1, 10);
        _userRepository.GetAllAsync(null, null, 1, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(emptyPaginatedList));

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Data);
        Assert.Equal(0, result.Data.TotalItems);
    }
    
    [Fact]
    public async Task Handle_Should_Return_Paginated_Users_When_They_Exist()
    {
        var query = new GetUsersQuery
        {
            Filter = null,
            OrderBy = "Username",
            Page = 1,
            Size = 10
        };

        var users = new List<User>
        {
            new User("user1@example.com", "user1", "hashedPassword",
                new Name("John", "Doe"), new Address("City", "Street", 100, "12345", new Geolocation("12.34", "56.78")),
                "999888777", UserRole.Admin, UserStatus.Active),

            new User("user2@example.com", "user2", "hashedPassword",
                new Name("Jane", "Doe"), new Address("City", "Street", 101, "54321", new Geolocation("13.34", "57.78")),
                "888777666", UserRole.Customer, UserStatus.Active)
        };

        var paginatedList = new PaginatedList<User>(users, 2, 1, 10);
        _userRepository.GetAllAsync(null, "Username", 1, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedList));


        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.TotalItems);
        Assert.Equal(2, result.Data.Data.ToList().Count);
        Assert.Equal("user1@example.com", result.Data.Data.ToList()[0].Email);
        Assert.Equal("user2@example.com", result.Data.Data.ToList()[1].Email);
    }
    
    [Fact]
    public async Task Handle_Should_Call_Repository_With_Correct_Parameters()
    {
        var query = new GetUsersQuery
        {
            Filter = "Role==Admin",
            OrderBy = "Username",
            Page = 1,
            Size = 10
        };

        _userRepository.GetAllAsync("Role==Admin", "Username", 1, 10, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PaginatedList<User>(new List<User>(), 0, 1, 10)));

        await _handler.Handle(query, CancellationToken.None);

        await _userRepository.Received(1).GetAllAsync("Role==Admin", "Username", 1, 10, Arg.Any<CancellationToken>());
    }
    
}