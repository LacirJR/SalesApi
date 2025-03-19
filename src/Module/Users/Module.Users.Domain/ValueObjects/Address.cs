using Microsoft.EntityFrameworkCore;
using Module.Users.Domain.Entities;

namespace Module.Users.Domain.ValueObjects;

[Owned]
public class Address
{
    public string City { get; private set; }
    public string Street { get; private set; }
    public int Number { get; private set; }
    public string Zipcode { get; private set; }
    public Geolocation Geolocation { get; private set; }
    
    private Address() { }
    
    public Address(string city, string street, int number, string zipcode, Geolocation geolocation)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(zipcode))
            throw new ArgumentException("Address fields cannot be empty");

        City = city;
        Street = street;
        Number = number;
        Zipcode = zipcode;
        Geolocation = geolocation;
    }
}