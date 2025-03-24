using Microsoft.EntityFrameworkCore;
using Module.Sales.Tests.Unit.Fakes;

namespace Module.Sales.Tests.Unit.Fixtures;

public class DbContextFixture
{
    public FakeSaleDbContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<FakeSaleDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FakeSaleDbContext(options);
    }
}