using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Module.Users.Domain.ValueObjects;

[Owned]
public sealed class Geolocation
{
    public string Lat { get;  }
    public string Long { get;  }
    
    private Geolocation() { }

    public Geolocation(string latitude, string longitude)
    {

        Lat = latitude;
        Long = longitude;
    }
}