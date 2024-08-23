using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Tsundoku.ViewModels;
using Tsundoku.Views;
using ZXing.Net.Maui.Controls;

namespace Tsundoku
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseBarcodeReader();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<RegisterPageView>();
            builder.Services.AddSingleton<RegisterPageViewModel>();

            builder.Services.AddTransient<CameraPageView>();
            builder.Services.AddTransient<CameraPageViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
