using Bogus;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Module.Carts.Infrastructure.Persistence.Repositories;
using Module.Carts.Tests.Unit.Fixtures;
using Xunit;

namespace Module.Carts.Tests.Unit.Repositories;

public class CartRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly ICartDbContext _context;
    private readonly ICartRepository _repository;
    private readonly Faker _faker;

    public CartRepositoryTests(DbContextFixture fixture)
    {
        _context = fixture.CreateNewContext();
        _repository = new CartRepository(_context);
        _faker = new Faker();
    }

    [Fact]
    public async Task Should_Add_And_Get_Cart_By_Id()
    {
        var cart = new Cart(_faker.Random.Guid(), DateTime.UtcNow);
        await _repository.AddAsync(cart, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(cart.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(cart.Id, result!.Id);
    }

    [Fact]
    public async Task Should_Return_Cart_By_UserId()
    {
        var userId = _faker.Random.Guid();
        var cart = new Cart(userId, DateTime.UtcNow);

        await _repository.AddAsync(cart, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetCartByUserIdAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
    }

    [Fact]
    public async Task Should_Update_Cart()
    {
        var cart = new Cart(_faker.Random.Guid(), DateTime.UtcNow);
        await _repository.AddAsync(cart, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        cart.RemoveItem(Guid.NewGuid()); // Simula alguma modificação

        _repository.Update(cart);
        await _context.SaveChangesAsync(CancellationToken.None);

        var updated = await _repository.GetByIdAsync(cart.Id, CancellationToken.None);
        Assert.NotNull(updated);
    }

    [Fact]
    public async Task Should_Delete_Cart_By_Id()
    {
        var cart = new Cart(_faker.Random.Guid(), DateTime.UtcNow);
        await _repository.AddAsync(cart, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        await _repository.RemoveByIdAsync(cart.Id, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var deleted = await _repository.GetByIdAsync(cart.Id, CancellationToken.None);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Should_Return_Filtered_And_Paginated_Carts()
    {
        var userId = _faker.Random.Guid();

        for (int i = 0; i < 10; i++)
        {
            var cart = new Cart(userId, DateTime.UtcNow.AddDays(-i));
            await _repository.AddAsync(cart, CancellationToken.None);
        }

        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(null, "Date desc", 1, 5, CancellationToken.None);

        Assert.Equal(5, result.Data.ToList().Count);
        Assert.Equal(10, result.TotalCount);
    }
}
