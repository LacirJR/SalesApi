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

    public static User Create(string email, string username, string password, Name name, Address address, string phone, UserRole role)
    {
        var user = new User(email, username, password, name, address, phone, role);
        return user;
    }
    
    public User(string email, string username, string password, Name name, Address address, string phone, UserRole role)
    {
        Id = Guid.NewGuid();
        Email = email;
        Username = username;
        Password = PasswordHash.Create(password).Value;
        Name = name;
        Address = address;
        Phone = phone;
        Role = role;
    }

    public void ChangeAddress(Address newAddress)
    {
        if(newAddress == null)
            throw new ArgumentException("Address cannot be null");
        Address = newAddress;
    }
    
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }
    
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrEmpty(newPasswordHash))
            throw new ArgumentException("Password cannot be empty");

        Password = newPasswordHash;
    }
    
}
