namespace GoneDotNet.WhereAreYou.Data;

public class UserCheckin
{
    public Guid Id { get; set; }
    public string UserIdentifier { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Heading { get; set; }
    public double Speed { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}