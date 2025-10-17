namespace GoneDotNet.WhereAreYou.Maui.Services;


[Singleton]
public record BaseServices(
    IConfiguration Configuration,
    INavigator Navigator,
    ILoggerFactory LoggerFactory
);