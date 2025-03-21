using Microsoft.EntityFrameworkCore;
using Module.Products.Tests.Unit.Fakes;

namespace Module.Products.Tests.Unit.Fixtures;

public class DbContextFixture
{
    public FakeProductsDbContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<FakeProductsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FakeProductsDbContext(options);
    }
}