using Microsoft.EntityFrameworkCore;
using Module.Carts.Tests.Unit.Fakes;

namespace Module.Carts.Tests.Unit.Fixtures;


public class DbContextFixture
{
    public FakeCartsDbContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<FakeCartsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FakeCartsDbContext(options);
    }
}