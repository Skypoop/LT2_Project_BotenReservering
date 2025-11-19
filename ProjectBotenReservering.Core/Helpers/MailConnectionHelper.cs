using Microsoft.Extensions.Configuration;

namespace ProjectBotenReservering.Core.Helpers
{
    public static class MailConnectionHelper
    {
        public static string? MailConnectionStringValue(string name)
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            IConfigurationSection section = config.GetSection("mailConnection");

            return section.GetValue<string>(name);
        }
    }
}