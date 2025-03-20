namespace Module.Users.Tests.Unit.Fakes;

public class FakeDateTimeProvider
{
    public DateTime Now => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
