using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProjectBotenReservering.App.Context;
using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Repositories;
using ProjectBotenReservering.Core.Services;
using ProjectBotenReservering.Core.Data.Services;
using ProjectBotenReservering.Core.Interfaces.Context;

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

            builder.Services.AddSingleton<IBoatRepository>(sp => BoatRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IClientRepository>(sp => ClientRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IReservationRepository>(sp => ReservationRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IRoleRepository>(sp => RoleRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IManagementTaskRepository>(sp => ManagementTaskRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IDamageReportRepository>(sp => DamageReportRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IDamageReportPhotoRepository>(sp => DamageReportPhotoRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IWindConstraintRepository>(sp => WindConstraintRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IClientReservationRepository>(sp => ClientReservationRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IClientRoleRepository>(sp => ClientRoleRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IClientManagementTaskRepository>(sp => ClientManagementTaskRepository.CreateAsync().GetAwaiter().GetResult());
            builder.Services.AddSingleton<IRoleManagementTaskRepository>(sp => RoleManagementTaskRepository.CreateAsync().GetAwaiter().GetResult());

            builder.Services.AddSingleton<IMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IBoatTypeService, BoatTypeService>();
            builder.Services.AddSingleton<IBoatAuthorizationService, BoatAuthorizationService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            
            builder.Services.AddSingleton<IClientContext, HardcodedClientContext>();
            
            builder.Services.AddTransient<HomePageView>().AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<BoatTypesView>().AddTransient<BoatTypesViewModel>();
          

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
