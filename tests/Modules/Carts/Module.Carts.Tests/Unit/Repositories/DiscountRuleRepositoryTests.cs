using Bogus;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using Module.Carts.Infrastructure.Persistence.Repositories;
using Module.Carts.Tests.Unit.Fixtures;
using NSubstitute;
using Xunit;

namespace Module.Carts.Tests.Unit.Repositories;

public class DiscountRuleRepositoryTests : IClassFixture<DbContextFixture>
{
    private readonly ICartDbContext _dbContext;
    private readonly IDiscountRuleRepository _repository;
    private readonly Faker _faker;

    public DiscountRuleRepositoryTests(DbContextFixture fixture)
    {
        _dbContext = fixture.CreateNewContext();
        _repository = new DiscountRuleRepository(_dbContext);
        _faker = new Faker();
    }

    [Fact]
    public async Task Should_Return_Only_Active_DiscountRules()
    {
        var activeRule = new DiscountRule(1, 5, 10, true);
        var inactiveRule = new DiscountRule(6, 10, 20, false);

        await _dbContext.DiscountRules.AddRangeAsync(activeRule, inactiveRule);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.GetActiveRulesAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Contains(result, r => r.Active);
    }

    [Fact]
    public async Task Should_Return_Empty_When_No_Active_DiscountRules()
    {
        var rule1 = new DiscountRule(1, 5, 10, false);
        var rule2 = new DiscountRule(6, 10, 20, false);

        await _dbContext.DiscountRules.AddRangeAsync(rule1, rule2);
        await _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>());

        var result = await _repository.GetActiveRulesAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Should_Return_Empty_When_No_DiscountRules_Exist()
    {
        var result = await _repository.GetActiveRulesAsync(CancellationToken.None);

        Assert.Empty(result);
    }
}
