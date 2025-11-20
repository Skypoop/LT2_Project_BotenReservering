using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.Text.Json;

namespace ProjectBotenReservering.Core.Service
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast");
        }

        public async Task<int> GetWeatherAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("?latitude=52.51&longitude=6.09&current_weather=true");
            response.EnsureSuccessStatusCode();
            string jsonResponse = await response.Content.ReadAsStringAsync();

            WeatherData? weatherData = JsonSerializer.Deserialize<WeatherData>(jsonResponse);

            decimal windspeedKmh = weatherData.CurrentWeather.Windspeed;

            int windforce = WindforceService.GetWindforce(windspeedKmh);
            return windforce;
        }
    }
}

  