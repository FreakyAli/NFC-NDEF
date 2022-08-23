#if ANDROID||iOS
using NDEF.MAUI.Platforms;
#endif
using NDEF.MAUI.ViewModels;

namespace NDEF.MAUI;

public static class MauiProgram
{
    public static MauiAppBuilder builder;

    public static MauiApp CreateMauiApp()
    {
        builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID||iOS
        builder.Services.AddTransient<Interfaces.INfcService, NfcService>();
#endif
        builder.Services.AddTransient<MainViewModel>();
        return builder.Build();
    }
}

