using Microsoft.EntityFrameworkCore;

namespace Module.Users.Domain.ValueObjects;

[Owned]
public class Name
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    
    private Name() { }

    public Name(string firstname, string lastname)
    {
        if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
            throw new ArgumentException("Firstname and Lastname cannot be empty");

        Firstname = firstname;
        Lastname = lastname;
    }

    public override string ToString() => $"{Firstname} {Lastname}";
}