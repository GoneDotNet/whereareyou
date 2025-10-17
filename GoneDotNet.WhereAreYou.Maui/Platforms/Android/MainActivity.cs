using Android.App;
using Android.OS;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;

namespace GoneDotNet.WhereAreYou.Maui;


[Activity(
    LaunchMode = LaunchMode.SingleTop,
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density
)]
[IntentFilter(
    [
        ShinyNotificationIntents.NotificationClickAction,
        Platform.Intent.ActionAppAction,
        global::Android.Content.Intent.ActionView
    ],
    Categories =
    [
        global::Android.Content.Intent.CategoryDefault,
        global::Android.Content.Intent.CategoryBrowsable
    ]
)]
public class MainActivity : MauiAppCompatActivity
{
}