using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ContactApp.Services;
using ContactApp.Views;
using ContactApp.ViewModels;
using ZXing.Net.Maui.Controls;

namespace ContactApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
            .UseMauiApp<App>()
            // Initialize the ZXing.Net.MAUI library by adding the below line of code
            .UseBarcodeReader()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            // After initializing the .NET MAUI Community Toolkit, optionally add additional fonts
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            builder.Services.AddSingleton(sp => new HttpClient 
                { 
                    BaseAddress = new Uri(Constants.BaseUrl), 
                    Timeout = TimeSpan.FromSeconds(30) 
                }
            );
            builder.Services.AddSingleton<ContactService>();
            builder.Services.AddSingleton<ContactsViewModel>();
            builder.Services.AddTransient<ContactPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
