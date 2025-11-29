using System.Text.Json.Serialization;

namespace ProjectBotenReservering.Core.Models
{
    public class WeatherData
    {
        [JsonPropertyName("current_weather")]
        public required CurrentWeather CurrentWeather { get; set; }

        [JsonPropertyName("hourly")]
        public required HourlyData Hourly { get; set; }
    }
}
