using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace ProjectBotenReservering.Core.Helpers
{
    public static class MailConnectionHelper
    {
        public static string? mailConnectionStringValue(string name)
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            IConfigurationSection section = config.GetSection("SMTP");

            return section.GetValue<string>(name);
        }
    }
}