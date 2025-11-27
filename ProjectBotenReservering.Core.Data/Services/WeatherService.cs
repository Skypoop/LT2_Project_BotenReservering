using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Services;
using System.Text.Json;

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
            int index = timeList.FindIndex(str => str == formattedDateTime);

            if (index == -1)
                throw new InvalidOperationException($"Datum {formattedDateTime} niet gevonden in weather data.");

            return index;
        }

        private static decimal GetMaxWindSpeedInRange(List<decimal> windSpeeds, int startIndex, int endIndex)
        {
            int count = endIndex - startIndex + 1;

            if (count <= 0)
                throw new InvalidOperationException("Ongeldige datum range: eindtijd moet na starttijd zijn.");

            return windSpeeds.Skip(startIndex).Take(count).Max();
        }

        public async Task<int> GetWeatherAsync(DateTime? beginDate = null, DateTime? endDate = null)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                "?latitude=52.52&longitude=13.41&current=wind_speed_10m&hourly=wind_speed_10m&current_weather=true"
            );
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse);

            if (weatherData?.CurrentWeather == null)
                throw new InvalidOperationException("Weather data is missing.");

            if (weatherData?.Hourly == null)
                throw new InvalidOperationException("Hourly weather data is missing.");

            // Format datums
            string startDateFormatted = FormatDateTime(beginDate.Value);
            string endDateFormatted = FormatDateTime(endDate.Value);

            // Vind indexes
            int startDateIndex = GetDateIndex(weatherData.Hourly.Time, startDateFormatted);
            int endDateIndex = GetDateIndex(weatherData.Hourly.Time, endDateFormatted);

            // Bereken max windsnelheid in range
            decimal maxWindSpeed = GetMaxWindSpeedInRange(weatherData.Hourly.WindSpeed10m, startDateIndex, endDateIndex);

            int windforce = WindforceService.GetWindforce(maxWindSpeed);
            return windforce;
        }
    }
}
