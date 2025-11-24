using Microsoft.Extensions.Configuration;

namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class ConnectionHelper
    {
        public static string? ConnectionStringValue(string name)
        {
            string resourcePath = "";
#if MACCATALYST
                resourcePath = Path.Combine(AppContext.BaseDirectory, "..", "Resources", "appsettings.json");
                resourcePath = Path.GetFullPath(resourcePath);
#else
            resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
#endif

            if (!File.Exists(resourcePath))
            {
                Console.WriteLine($"appsettings.json NOT found at {resourcePath}");
                return null;
            }

                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(resourcePath)!)
                    .AddJsonFile(Path.GetFileName(resourcePath)!, optional: false, reloadOnChange: true)
                    .Build();

            IConfigurationSection section = config.GetSection("ConnectionStrings");
            return section.GetValue<string>(name);
        }
    }
}
