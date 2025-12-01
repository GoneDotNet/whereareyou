using GoneDotNet.WhereAreYou.Grains.Interfaces;

namespace GoneDotNet.WhereAreYou.Grains.Impl;


public class CompanyGrain(
    [PersistentState("company")] 
    IPersistentState<CompanyState> state
) : Grain, ICompanyGrain
{
    public async Task<List<Location>> GetLastDriverLocations()
    {
        var states = new List<Location>();
        foreach (var driverId in state.State.Drivers)
        {
            var driverGrain = this.GrainFactory.GetGrain<IDriverGrain>(driverId);
            var driverState = await driverGrain.GetState();
            if (driverState.LastKnownLocation != null)
            {
                var loc = driverState.LastKnownLocation;
                loc.DriverName = driverId;
                states.Add(loc);
            }
        }
        return states;
    }


    public async Task Join(string driverId)
    {
        if (state.State.Drivers.Contains(driverId))
            return;
        
        state.State.Drivers.Add(driverId);
        await state.WriteStateAsync();
    }


    public async Task Leave(string driverId)
    {
        if (state.State.Drivers.Remove(driverId))
            await state.WriteStateAsync();
    }
}

[GenerateSerializer]
public class CompanyState
{
    [Id(0)]
    public List<string> Drivers { get; set; } = new();
}