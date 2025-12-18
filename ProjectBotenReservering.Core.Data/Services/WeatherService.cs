using System.Text.Json;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;

namespace ProjectBotenReservering.Core.Data.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast");
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm");
        }

        private static int GetDateIndex(List<string> timeList, string formattedDateTime)
        {
            return timeList.FindIndex(str => str == formattedDateTime);
        }

        // Returns the maximum wind speed between the specified start and end indices (inclusive).
        // The startIndex and endIndex define the range within the windSpeeds list.
        private static decimal GetMaxWindSpeedInRange(List<decimal> windSpeeds, int startIndex, int endIndex)
        {
            try
            {
                int indexRange = endIndex - startIndex + 1;

                if (indexRange <= 0)
                {
                    throw new InvalidOperationException($"Ongeldige index range: index range moet altijd 1 of hoger zijn, index was: {indexRange}");
                }

                return windSpeeds.Skip(startIndex).Take(indexRange).Max();
            } catch(InvalidOperationException exception)
            {
                Console.WriteLine(exception.Message);

                return 0;
            }
        }

        public async Task<int> GetWeatherAsync(DateTime beginDate, DateTime endDate)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("?latitude=52.52&longitude=13.41&current=wind_speed_10m&hourly=wind_speed_10m&current_weather=true");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse);

                if (weatherData == null || weatherData.Hourly == null || weatherData.Hourly.Time == null || weatherData.Hourly.WindSpeed10m == null)
                {
                    Console.WriteLine("Weather data or required properties are null.");

                    return 0;
                }

                string startDateFormatted = FormatDateTime(beginDate);
                string endDateFormatted = FormatDateTime(endDate);

                int startDateIndex = GetDateIndex(weatherData.Hourly.Time, startDateFormatted);
                int endDateIndex = GetDateIndex(weatherData.Hourly.Time, endDateFormatted);

                decimal maxWindSpeed = GetMaxWindSpeedInRange(weatherData.Hourly.WindSpeed10m, startDateIndex, endDateIndex);

                int windforce = WindforceService.GetWindforce(maxWindSpeed);

                return windforce;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                return 0;
            }
        }
    }
}
