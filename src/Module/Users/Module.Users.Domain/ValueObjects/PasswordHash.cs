namespace Module.Users.Domain.ValueObjects;

public sealed class PasswordHash
{
    public string Value { get; }

    private PasswordHash(string hash)
    {
        Value = hash;
    }

    public static PasswordHash Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password must have at least 6 characters.");

        var hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        return new PasswordHash(hash);
    }

    public bool Verify(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Value);
    }
}