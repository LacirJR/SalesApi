using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common.Enums;

namespace Module.Users.Application.Dtos;

public class FakeUser
{
    public static User CreateFakeUser(string email = "valid@example.com", string password = "correctpassword")
    {
        var user = new User(
            email,
            "validuser",
            password,  // 🔥 Simulamos uma senha já armazenada como hash
            new Name("John", "Doe"),
            new Address("City", "Street", 100, "12345", new Geolocation("12.34", "56.78")),
            "999888777",
            UserRole.Admin,
            UserStatus.Active
        );

        return user;
    }
}
