using GoneDotNet.WhereAreYou.Api;
using GoneDotNet.WhereAreYou.Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddKeyedRedisClient("redis");
builder.UseOrleans(silo =>
{
    silo.UseDashboard(x => x.Port = 1911);
});

var app = builder.Build();
app.MapPost(
    "/gps",
    async (
        [FromBody] GpsPing gpsPing,
        [FromServices] IGrainFactory grainFactory
    ) =>
    {
        var driverGrain = grainFactory.GetGrain<IDriverGrain>(gpsPing.DriverName);
        await driverGrain.UpdateLocation(new Location
        {
            Latitude = gpsPing.Latitude,
            Longitude = gpsPing.Longitude,
            Timestamp = gpsPing.Timestamp
        });
    }
);
app.Run();
