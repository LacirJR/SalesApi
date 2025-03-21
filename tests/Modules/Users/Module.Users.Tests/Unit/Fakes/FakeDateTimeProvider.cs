using Bogus;

namespace Module.Users.Tests.Unit.Fakes;

public class FakeDateTimeProvider
{
    public DateTime Now { get; }

    public FakeDateTimeProvider()
    {
        var faker = new Faker();
        Now = faker.Date.Between(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
    }
}

