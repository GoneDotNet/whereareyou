using GoneDotNet.WhereAreYou.Data;
using GoneDotNet.WhereAreYou.Grains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Orleans.Streams;

namespace GoneDotNet.WhereAreYou.Grains.Impl;


public class DriverGrain(
    [PersistentState("driver")] 
    IPersistentState<DriverState> state,
    IDbContextFactory<AppDbContext> dbContextFactory
) : Grain, IDriverGrain
{
    private IAsyncStream<Location>? stream;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var companyGrain = this.GrainFactory.GetGrain<ICompanyGrain>("gonedotnet");
        await companyGrain.Join(this.GetPrimaryKeyString());
        
        var streamProvider = this.GetStreamProvider("StreamProvider");
        stream = streamProvider.GetStream<Location>(StreamId.Create("drivers", "all"));
        await base.OnActivateAsync(cancellationToken);
    }

    
    public async Task UpdateLocation(Location location)
    {
        state.State.LastKnownLocation = location;
        await state.WriteStateAsync();

        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.UserCheckins.Add(new UserCheckin
        {
            Id = Guid.NewGuid(),
            Heading = location.Heading,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Speed = location.Speed,
            Timestamp = location.Timestamp,
            UserIdentifier = this.GetPrimaryKeyString()
        });
        await db.SaveChangesAsync();
        
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