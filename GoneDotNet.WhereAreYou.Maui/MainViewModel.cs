using GoneDotNet.WhereAreYou.Maui.Delegates;
using Shiny.Locations;

namespace GoneDotNet.WhereAreYou.Maui;


[ShellMap<MainPage>]
public partial class MainViewModel(
    IGpsManager gpsManager,
    MyGpsDelegate gpsDelegate,
    INavigator navigator
) : ObservableObject
{
    [ObservableProperty] public partial string DriverName { get; set; }
    public bool IsWorking => gpsManager.CurrentListener != null;

    [RelayCommand]
    async Task ChangeStatus()
    {
        try
        {
            if (gpsManager.CurrentListener == null)
            {
                gpsDelegate.DriverName = this.DriverName;
                await gpsManager.StartListener(GpsRequest.Realtime(true));
            }
            else
            {
                await gpsManager.StopListener();
            }
            this.OnPropertyChanged(nameof(IsWorking));
        }
        catch (Exception ex)
        {
            await navigator.Alert(
                "An error occurred while changing GPS status. Please try again.",
                "Error",
                "OK"
            );
        }
    }
    
    bool CanChangeStatus() => gpsManager.CurrentListener != null || 
                              !String.IsNullOrWhiteSpace(this.DriverName);
}