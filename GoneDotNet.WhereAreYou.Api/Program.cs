using GoneDotNet.WhereAreYou.Api;
using GoneDotNet.WhereAreYou.Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans.Streams;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddOrleansClient();

var app = builder.Build();
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
            Timestamp = gpsPing.Timestamp
        });
    }
);

app.MapGet(
    "/drivers",
    ([FromServices] IClusterClient orleansClient) =>
    {
        return StreamDriverUpdates(orleansClient);
    }
);


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
            yield return new GpsPing(ping.DriverName!, ping.Latitude, ping.Longitude, ping.Timestamp);
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
