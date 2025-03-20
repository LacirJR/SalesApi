using Microsoft.EntityFrameworkCore;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common;
using Shared.Domain.Common.Enums;

namespace Module.Users.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Email { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public Name Name { get; private set; }
    public Address Address { get; private set; }
    public string Phone { get; private  set; }
    public UserStatus Status { get; private set; }
    public UserRole Role { get; private set; }

    private User(){}

    public User(string email, string username, string password, Name name, Address address, string phone, UserRole role, UserStatus status)
    {
        Id = Guid.NewGuid();
        Email = email;
        Username = username;
        Password = PasswordHash.Create(password).Value;
        Name = name;
        Address = address;
        Phone = phone;
        Role = role;
        Status = Status;
    }
    
    public static User Create(string email, string username, string password, Name name, Address address, string phone, UserRole role, UserStatus status)
    {
        var user = new User(email, username, password, name, address, phone, role, status);
        return user;
    }



    public void Update(string email, string username, string password, Name name, Address address, string phone, UserRole role, UserStatus status)
    {
        Email = email;
        Username = username;
        Name = name;
        Address = address;
        Phone = phone;
        Role = role;
        Status = status;
        
        if(password != Password)
            Password = PasswordHash.Create(password).Value;
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHash.Verify(password, Password);
    }
    
}
