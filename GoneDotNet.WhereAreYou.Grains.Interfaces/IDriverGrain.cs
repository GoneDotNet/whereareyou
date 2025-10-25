﻿namespace GoneDotNet.WhereAreYou.Grains.Interfaces;


public interface IDriverGrain : IGrainWithStringKey
{
    Task UpdateLocation(Location location);
    
    Task<DriverState> GetState();
}

[GenerateSerializer]
public class DriverState
{
    [Id(0)]
    public Location? LastKnownLocation { get; set; }
}


[GenerateSerializer]
public class Location
{
    [Id(0)]
    public double Latitude { get; set; }
    
    [Id(1)]
    public double Longitude { get; set; }
    
    [Id(2)]
    public DateTimeOffset Timestamp { get; set; }
}