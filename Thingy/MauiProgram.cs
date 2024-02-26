
namespace Thingy;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
        .CreateBuilder()
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .UseShinyFramework(
            new DryIocContainerExtension(),
            prism => prism.OnAppStart("NavigationPage/BleScan"),
            new (
#if DEBUG
                ErrorAlertType.FullError
#else
                ErrorAlertType.NoLocalize
#endif
            )
        )
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        })
        .RegisterInfrastructure()
        .RegisterAppServices()
        .RegisterViews()
        .Build();


    static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder) 
    {
        builder.Services.AddBluetoothLE();
        return builder;
    }


    static MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
    {
        builder.Configuration.AddJsonPlatformBundle();
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif
        var s = builder.Services;
        s.AddDataAnnotationValidation();
        return builder;
    }


    static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        var s = builder.Services;


        s.RegisterForNavigation<MainPage, MainViewModel>();
        s.RegisterForNavigation<ScanPage, ScanViewModel>("BleScan");
        s.RegisterForNavigation<PeripheralPage, PeripheralViewModel>("BlePeripheral");
        s.RegisterForNavigation<ServicePage, ServiceViewModel>("BlePeripheralService");
        s.RegisterForNavigation<CharacteristicPage, CharacteristicViewModel>("BlePeripheralCharacteristic");
        return builder;
    }
}

