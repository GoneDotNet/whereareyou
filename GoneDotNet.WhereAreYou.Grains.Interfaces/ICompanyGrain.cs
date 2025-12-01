namespace GoneDotNet.WhereAreYou.Grains.Interfaces;


public interface ICompanyGrain : IGrainWithStringKey
{
    Task<List<DriverState>> GetAllDriverStates();
    Task Join(string driverId);
    Task Leave(string driverId);
}