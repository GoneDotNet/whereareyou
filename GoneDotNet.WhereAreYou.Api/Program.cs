using GoneDotNet.WhereAreYou.Api;
using GoneDotNet.WhereAreYou.Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans.Streams;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddOrleansClient();
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost(
    "/gps",
    async (
        [FromBody] GpsPing gpsPing,
        [FromServices] IClusterClient orleansClient
    ) =>
    {
        var driverGrain = orleansClient.GetGrain<IDriverGrain>(gpsPing.DriverName);
        await driverGrain.UpdateLocation(new Location
        {
            Latitude = gpsPing.Latitude,
            Longitude = gpsPing.Longitude,
            Heading = gpsPing.Heading,
            Speed = gpsPing.Speed,
            Timestamp = gpsPing.Timestamp
        });
    }
);

app.MapGet(
    "/drivers",
    // the .net 10 way
    ([FromServices] IClusterClient orleansClient) => TypedResults.ServerSentEvents(StreamDriverUpdates(orleansClient))
        
    // pre-net10
    // context.Response.Headers["Content-Type"] = "text/event-stream";
    // context.Response.Headers["Cache-Control"] = "no-cache";
    // context.Response.Headers["Connection"] = "keep-alive";
    //
    // await foreach (var gpsPing in StreamDriverUpdates(orleansClient))
    // {
    //     var json = JsonSerializer.Serialize(gpsPing);
    //     await context.Response.WriteAsync($"data: {json}\n\n");
    //     await context.Response.Body.FlushAsync();
    // }
);

app.MapGet(
    "companies/{companyId}",
    async (
        [FromRoute] string companyId,
        [FromServices] IClusterClient orleansClient
    ) =>
    {
        var companyGrain = orleansClient.GetGrain<ICompanyGrain>(companyId);
        var states = await companyGrain.GetLastDriverLocations();
        var result = states
            .Select(x => new GpsPing(
                x.DriverName!,
                x.Latitude,
                x.Longitude,
                x.Heading,
                x.Speed,
                x.Timestamp
            ))
            .ToList();
        
        return Results.Ok(result);
    }
);

app.UseStaticFiles();
app.Run();

async IAsyncEnumerable<GpsPing> StreamDriverUpdates(IClusterClient client)
{
    var streamProvider = client.GetStreamProvider("StreamProvider");
    var streamId = StreamId.Create("drivers", "all");
    var stream = streamProvider.GetStream<Location>(streamId);

    var channel = System.Threading.Channels.Channel.CreateUnbounded<Location>();
    
    var observer = new GpsPingObserver(channel.Writer);
    var sub = await stream.SubscribeAsync(observer);

    try
    {
        await foreach (var ping in channel.Reader.ReadAllAsync())
        {
            yield return new GpsPing(
                ping.DriverName!, 
                ping.Latitude, 
                ping.Longitude,
                ping.Heading,
                ping.Speed,
                ping.Timestamp
            );
        }
    }
    finally
    {
        await sub.UnsubscribeAsync();
        channel.Writer.Complete();
    }
}

class GpsPingObserver : IAsyncObserver<Location>
{
    private readonly System.Threading.Channels.ChannelWriter<Location> _writer;

    public GpsPingObserver(System.Threading.Channels.ChannelWriter<Location> writer)
    {
        _writer = writer;
    }

    public Task OnNextAsync(Location item, StreamSequenceToken? token = null)
    {
        return _writer.WriteAsync(item).AsTask();
    }

    public Task OnCompletedAsync()
    {
        _writer.Complete();
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _writer.Complete(ex);
        return Task.CompletedTask;
    }
}
