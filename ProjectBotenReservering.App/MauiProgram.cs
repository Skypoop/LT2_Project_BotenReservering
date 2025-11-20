using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Data.Services;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Services;

namespace ProjectBotenReservering.App
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
                });


            builder.Services.AddTransient<HomePageView>().AddTransient<HomePageViewModel>();
            // Services registreren
            builder.Services.AddSingleton<IMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();

            // ViewModels en Views registreren
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePageView>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
