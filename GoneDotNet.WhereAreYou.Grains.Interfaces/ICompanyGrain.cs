namespace GoneDotNet.WhereAreYou.Grains.Interfaces;


public interface ICompanyGrain : IGrainWithStringKey
{
    Task<List<Location>> GetLastDriverLocations();
    Task Join(string driverId);
    Task Leave(string driverId);
}