using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Module.Users.Domain.ValueObjects;

[Owned]
public sealed class Geolocation
{
    public string Lat { get;  }
    public string Long { get;  }
    
    private Geolocation() { }

    public Geolocation(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");

        Lat = latitude.ToString(CultureInfo.InvariantCulture);
        Long = longitude.ToString(CultureInfo.InvariantCulture);
    }
}