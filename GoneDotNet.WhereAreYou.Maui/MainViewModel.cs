using Shiny.Locations;

namespace GoneDotNet.WhereAreYou.Maui;


[ShellMap<MainPage>]
public partial class MainViewModel(IGpsManager gpsManager) : ObservableObject
{
    [ObservableProperty] public partial string DriverName { get; set; }
    public bool IsWorking => gpsManager.CurrentListener != null;

    [RelayCommand(CanExecute = nameof(CanChangeStatus))]
    async Task ChangeStatus()
    {
        try
        {
            if (gpsManager.CurrentListener == null)
            {
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
            // services.Logger.LogError(ex, "Error changing GPS status");
            // await services.Dialogs.Alert(
            //     "An error occurred while changing GPS status. Please try again.",
            //     "Error",
            //     "OK"
            // );
        }
    }
    
    bool CanChangeStatus() => gpsManager.CurrentListener != null || 
                              !String.IsNullOrWhiteSpace(this.DriverName);
}