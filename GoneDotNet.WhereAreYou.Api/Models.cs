namespace GoneDotNet.WhereAreYou.Api;


public record GpsPing(
    string DriverName,
    double Latitude, 
    double Longitude,
    double Heading,
    double Speed,
    DateTimeOffset Timestamp
);