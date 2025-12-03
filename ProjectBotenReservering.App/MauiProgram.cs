using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
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
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            MailSettings mailSettings = configuration.GetSection("MailConnection").Get<MailSettings>()!;
            IServiceCollection serviceCollection = builder.Services.AddSingleton(mailSettings);

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

            builder.Services.AddSingleton<ISmtpMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IBoatTypeService, BoatTypeService>();
            builder.Services.AddSingleton<IBoatAuthorizationService, BoatAuthorizationService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IReservationService, ReservationService>();
            
            builder.Services.AddSingleton<IClientContext, HardcodedClientContext>();
            
            builder.Services.AddTransient<HomePageView>().AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<BoatTypesView>().AddTransient<BoatTypesViewModel>();
            builder.Services.AddTransient<ReservationFormView>().AddTransient<ReservationFormViewModel>();
            builder.Services.AddTransient<SideBarView>().AddTransient<SideBarViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
