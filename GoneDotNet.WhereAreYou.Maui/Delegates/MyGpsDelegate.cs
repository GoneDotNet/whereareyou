using Shiny.Locations;

namespace GoneDotNet.WhereAreYou.Maui.Delegates;


public partial class MyGpsDelegate : GpsDelegate
{
    public MyGpsDelegate(ILogger<MyGpsDelegate> logger) : base(logger)
    {
        // settings as you need
        // this.MinimumDistance = Distance.FromMeters(200);
        // this.MinimumTime = TimeSpan.FromSeconds(15);
    }


    protected override async Task OnGpsReading(GpsReading reading)
    {
    }
}

#if ANDROID
public partial class MyGpsDelegate : IAndroidForegroundServiceDelegate
{
    public void Configure(AndroidX.Core.App.NotificationCompat.Builder builder)
    {
        
    }
}
#endif