using AutoMapper;
using Bogus;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Application.Queries.GetAllCartsQuery;
using Module.Carts.Domain.Entities;
using NSubstitute;
using Shared.Infrastructure.Common;
using Xunit;

namespace Module.Carts.Tests.Unit.Handlers;

public class GetAllCartsQueryHandlerTests
{
    private readonly Faker _faker;
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly GetAllCartsQueryHandler _handler;

    public GetAllCartsQueryHandlerTests()
    {
        _faker = new Faker();
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetAllCartsQueryHandler(_cartRepository, _mapper);
    }
    
    [Fact]
    public async Task Should_Return_Paginated_Carts()
    {
        var carts = new List<Cart>
        {
            new(_faker.Random.Guid(), _faker.Date.Past()),
            new(_faker.Random.Guid(), _faker.Date.Past())
        };

        var paginated = new PaginatedList<Cart>(carts, carts.Count, 1, 10);

        _cartRepository.GetAllAsync(Arg.Any<string?>(), Arg.Any<string?>(), 1, 10, Arg.Any<CancellationToken>())
            .Returns(paginated);

        _mapper.Map<List<CartResponseDto>>(carts)
            .Returns(carts.Select(c => new CartResponseDto { Id = c.Id }).ToList());

        var query = new GetAllCartsQuery(null, null, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Data.ToList().Count);
    }
    
    [Fact]
    public async Task Should_Return_Empty_When_No_Carts()
    {
        var paginated = new PaginatedList<Cart>(new List<Cart>(), 0, 1, 10);

        _cartRepository.GetAllAsync(Arg.Any<string?>(), Arg.Any<string?>(), 1, 10, Arg.Any<CancellationToken>())
            .Returns(paginated);

        _mapper.Map<List<CartResponseDto>>(Arg.Any<List<Cart>>())
            .Returns(new List<CartResponseDto>());

        var query = new GetAllCartsQuery(null, null, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Data);
    }

    
}