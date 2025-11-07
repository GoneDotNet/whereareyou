using GoneDotNet.WhereAreYou.Grains.Interfaces;

namespace GoneDotNet.WhereAreYou.Grains.Impl;


public class DriverGrain(
    [PersistentState("driver")] 
    IPersistentState<DriverState> state
) : Grain, IDriverGrain
{
    public Task UpdateLocation(Location location)
    {
        state.State.LastKnownLocation = location;
        return state.WriteStateAsync();
    }
    

    public Task<DriverState> GetState()
    {
        return Task.FromResult(state.State);
    }
}