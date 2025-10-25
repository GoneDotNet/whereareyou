namespace GoneDotNet.WhereAreYou.Api;


public record GpsPing(
    string DriverName,
    double Latitude, 
    double Longitude,
    DateTimeOffset Timestamp
);