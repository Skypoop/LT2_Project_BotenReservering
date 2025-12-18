using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectBotenReservering.App.Helpers;
using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Context;
using ProjectBotenReservering.Core.Data.Database;
using ProjectBotenReservering.Core.Data.Database.Fixtures;
using ProjectBotenReservering.Core.Data.Database.Schema;
using ProjectBotenReservering.Core.Data.Database.Seeders;
using ProjectBotenReservering.Core.Data.Mappers;
using ProjectBotenReservering.Core.Data.Repositories;
using ProjectBotenReservering.Core.Data.RestClients;
using ProjectBotenReservering.Core.Data.Services;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Helpers;
using ProjectBotenReservering.Core.Interfaces.Mappers;
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

            string writableDirectory;
            const string databaseName = "Roeivereniging.db";

#if MACCATALYST
            writableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#elif ANDROID
            writableDirectory = FileSystem.AppDataDirectory;
#else
            writableDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

            Directory.CreateDirectory(writableDirectory);
            string fullPath = Path.Combine(writableDirectory, databaseName);
            string connectionString = $"Data Source={fullPath}";

            MailSettings mailSettings = configuration.GetSection("MailConnection").Get<MailSettings>()!;
            builder.Services.AddSingleton(mailSettings);

            builder.Services.AddSingleton<IDbConnectionFactory>(provider =>
                new SqliteConnectionFactory(connectionString));
            builder.Services.AddSingleton<IDatabaseBootstrap, SqliteDatabaseBootstrap>();

            builder.Services.AddSingleton<IPromptHelper, PromptHelper>();

            builder.Services.AddSingleton<ISchemaInitializer, SqliteSchemaInitializer>();
            builder.Services.AddTransient<IDatabaseSeeder, BoatSeeder>();
            builder.Services.AddTransient<IDatabaseSeeder, RoleSeeder>();
            builder.Services.AddTransient<IDatabaseSeeder, WindConstraintSeeder>();
            builder.Services.AddTransient<IDatabaseSeeder, ManagementTaskSeeder>();
            builder.Services.AddTransient<IDatabaseSeeder, RoleManagementTaskSeeder>();
            builder.Services.AddTransient<IDatabaseSeeder, ClientSeeder>();
            builder.Services.AddTransient<IDatabaseFixture, ReservationFixture>();
            builder.Services.AddTransient<IDatabaseSeeder, ClientRoleSeeder>();

            builder.Services.AddSingleton<IMapper<Boat>, BoatMapper>();
            builder.Services.AddSingleton<IMapper<Client>, ClientMapper>();
            builder.Services.AddSingleton<IMapper<Reservation>, ReservationMapper>();
            builder.Services.AddSingleton<IMapper<Role>, RoleMapper>();
            builder.Services.AddSingleton<IMapper<ManagementTask>, ManagementTaskMapper>();
            builder.Services.AddSingleton<IMapper<DamageReport>, DamageReportMapper>();
            builder.Services.AddSingleton<IMapper<DamageReportPhoto>, DamageReportPhotoMapper>();
            builder.Services.AddSingleton<IMapper<WindConstraint>, WindConstraintMapper>();
            builder.Services.AddSingleton<IMapper<Competition>, CompetitionMapper>();
            builder.Services.AddSingleton<IMapper<ClientReservation>, ClientReservationMapper>();
            builder.Services.AddSingleton<IMapper<ClientRole>, ClientRoleMapper>();
            builder.Services.AddSingleton<IMapper<ClientManagementTask>, ClientManagementTaskMapper>();
            builder.Services.AddSingleton<IMapper<RoleManagementTask>, RoleManagementTaskMapper>();
            builder.Services.AddSingleton<IMapper<ReservationCompetition>, ReservationCompetitionMapper>();

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
            builder.Services.AddSingleton<ICompetitionRepository, CompetitionRepository>();
            builder.Services.AddSingleton<IReservationCompetitionRepository, ReservationCompetitionRepository>();
            builder.Services.AddSingleton<ITweetRepository, TweetRepository>();

            builder.Services.AddSingleton((IServiceProvider sp) =>
            {
                string? apiKey = configuration["GeminiApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("GeminiApiKey is missing in appsettings.json");
                }
                return new LlmRestClient(apiKey);
            });
            builder.Services.AddSingleton((IServiceProvider sp) =>
            {

                string? apiKey = configuration["X_API_KEY"];
                string? apiSecret = configuration["X_API_SECRET"];
                string? accessKey = configuration["X_ACCESS_KEY"];
                string? accessSecret = configuration["X_ACCESS_SECRET"];

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret) ||
                    string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(accessSecret))
                {
                    throw new InvalidOperationException("One or more X API keys are missing in appsettings.json");
                }

                return new TweetRestClient(apiKey, apiSecret, accessKey, accessSecret);
            });
            builder.Services.AddSingleton<ILlmRepository, LlmRepository>();

            builder.Services.AddSingleton<ISmtpMailService, SmtpMailService>();
            builder.Services.AddSingleton<IWeatherService, WeatherService>();
            builder.Services.AddSingleton<IBoatTypeService, BoatTypeService>();
            builder.Services.AddSingleton<IBoatAuthorizationService, BoatAuthorizationService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IReservationService, ReservationService>();
            builder.Services.AddSingleton<IClientContext, ClientContext>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ILlmService, LlmService>();
            builder.Services.AddSingleton<ICompetitionService, CompetitionService>();
            builder.Services.AddSingleton<ITweetService, TweetService>();

            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<TabItemToViewHelper>();
            builder.Services.AddTransient<BoatTypesView>().AddTransient<BoatTypesViewModel>();
            builder.Services.AddTransient<ReservationFormView>().AddTransient<ReservationFormViewModel>();
            builder.Services.AddTransient<SideBarView>().AddTransient<SideBarViewModel>();
            builder.Services.AddTransient<LoginView>().AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterView>().AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<CompetitionView>().AddTransient<CompetitionViewModel>();
            builder.Services.AddTransient<TweetCreationView>().AddTransient<TweetCreationViewModel>();
            builder.Services.AddTransient<BoatTypeSelectionCompetitionView>().AddTransient<BoatTypeSelectionCompetitionViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            MauiApp app = builder.Build();

            IDatabaseBootstrap dbBootstrap = app.Services.GetRequiredService<IDatabaseBootstrap>();
            dbBootstrap.Setup();

            return app;
        }
    }
}