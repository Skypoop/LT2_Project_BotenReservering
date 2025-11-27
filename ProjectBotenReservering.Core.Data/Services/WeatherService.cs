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

        public async Task<int> GetWeatherAsync(DateTime? beginDate = null, DateTime? endDate = null)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                "?latitude=52.52&longitude=13.41&current=wind_speed_10m&hourly=wind_speed_10m&current_weather=true"
            );

            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            string startDateFormattedString = beginDate.Value.ToString("yyyy-MM-ddTHH:mm");
            string endDateFormattedString = endDate.Value.ToString("yyyy-MM-ddTHH:mm");

            WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse);

            if (weatherData?.CurrentWeather == null)
                throw new InvalidOperationException("Weather data is missing.");

            if (weatherData?.Hourly == null)
                throw new InvalidOperationException("Weather data is missing.");

            int startDateIndex = weatherData.Hourly.Time.FindIndex(str => str == startDateFormattedString);
            int endDateIndex = weatherData.Hourly.Time.FindIndex(str => str == endDateFormattedString);

            decimal windspeedKmh = weatherData.Hourly.WindSpeed10m.Skip(startDateIndex).Take(endDateIndex).Max();

            int windforce = WindforceService.GetWindforce(windspeedKmh);

            return windforce;
        }
    }
}
