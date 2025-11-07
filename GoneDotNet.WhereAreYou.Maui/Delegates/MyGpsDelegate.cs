using System.Net.Http.Json;
using Shiny.Locations;

namespace GoneDotNet.WhereAreYou.Maui.Delegates;


public partial class MyGpsDelegate : GpsDelegate
{
    public string DriverName { get; set; }
    
    private readonly HttpClient httpClient = new();
    public MyGpsDelegate(ILogger<MyGpsDelegate> logger) : base(logger)
    {
        // settings as you need
        // this.MinimumDistance = Distance.FromMeters(200);
        // this.MinimumTime = TimeSpan.FromSeconds(15);
    }


    protected override async Task OnGpsReading(GpsReading reading)
    {
        await this.httpClient.PostAsJsonAsync(
            $"{Constants.ApiBaseUrl}gps",
            new GpsPing(
                this.DriverName,
                reading.Position.Latitude, 
                reading.Position.Longitude,
                reading.Timestamp
            )
        );
        
    }
}

public record GpsPing(
    string DriverName,
    double Latitude,
    double Longitude,
    DateTimeOffset Timestamp
);

#if ANDROID
public partial class MyGpsDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif