namespace GoneDotNet.WhereAreYou.Maui;


public partial class MainViewModel(BaseServices services) : ObservableObject
{
    [ObservableProperty] public partial string Property { get; set; }


    [RelayCommand]
    async Task DoSomething()
    {
    }
}