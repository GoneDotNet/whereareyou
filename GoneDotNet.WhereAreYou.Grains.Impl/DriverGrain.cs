using GoneDotNet.WhereAreYou.Grains.Interfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace GoneDotNet.WhereAreYou.Grains.Impl;


public class DriverGrain(
    [PersistentState("driver")] 
    IPersistentState<DriverState> state
) : Grain, IDriverGrain
{
    private IAsyncStream<Location>? stream;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("StreamProvider");
        stream = streamProvider.GetStream<Location>(StreamId.Create("drivers", "all"));
        return base.OnActivateAsync(cancellationToken);
    }

    
    public async Task UpdateLocation(Location location)
    {
        state.State.LastKnownLocation = location;
        await state.WriteStateAsync();
        
        if (stream != null)
        {
            location.DriverName = this.GetPrimaryKeyString();
            await stream.OnNextAsync(location);
        }
    }
    

    public Task<DriverState> GetState()
    {
        return Task.FromResult(state.State);
    }
}