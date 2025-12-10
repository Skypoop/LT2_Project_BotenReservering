using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectBotenReservering.App.Helpers;
using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Context;
using ProjectBotenReservering.Core.Data.Repositories;
using ProjectBotenReservering.Core.Data.Services;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

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

            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<TabItemToViewHelper>();
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
            builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
            builder.Services.AddSingleton<IReservationMatchRepository, ReservationMatchRepository>();

            builder.Services.AddSingleton<ISmtpMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IBoatTypeService, BoatTypeService>();
            builder.Services.AddSingleton<IBoatAuthorizationService, BoatAuthorizationService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IReservationService, ReservationService>();
            builder.Services.AddSingleton<IClientContext, ClientContext>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IMatchService, MatchService>();

            builder.Services.AddTransient<BoatTypesView>().AddTransient<BoatTypesViewModel>();
            builder.Services.AddTransient<ReservationFormView>().AddTransient<ReservationFormViewModel>();
            builder.Services.AddTransient<SideBarView>().AddTransient<SideBarViewModel>();
            builder.Services.AddTransient<LoginView>().AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterView>().AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<CompetitionView>().AddTransient<CompetitionViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
