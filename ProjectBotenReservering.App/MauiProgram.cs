using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Repositories;
using ProjectBotenReservering.Core.Services;
using ProjectBotenReservering.Core.Data.Services;

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

            builder.Services.AddSingleton<IBoatRepository, BoatRepository>();
            builder.Services.AddSingleton<IClientRepository, ClientRepository>();
            builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
            builder.Services.AddSingleton<IRoleRepository, RoleRepository>();
            builder.Services.AddSingleton<IManagementTaskRepository, ManagementTaskRepository>();
            builder.Services.AddSingleton<IDamageReportRepository, DamageReportRepository>();
            builder.Services.AddSingleton<IDamageReportPhotoRepository, DamageReportPhotoRepository>();
            builder.Services.AddSingleton<IWindConstraintRepository, WindConstraintRepository>();
            builder.Services.AddSingleton<IClientReservationRepository, ClientReservationRepository>();
            builder.Services.AddSingleton<IClientRoleRepository, ClientRoleRepository>();
            builder.Services.AddSingleton<IClientManagementTaskRepository, ClientManagementTaskRepository>();
            builder.Services.AddSingleton<IRoleManagementTaskRepository, RoleManagementTaskRepository>();

            builder.Services.AddSingleton<IMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IBoatTypeService, BoatTypeService>();
            
            builder.Services.AddTransient<HomePageView>().AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<BoatTypesView>().AddTransient<BoatTypesViewModel>();
          

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
