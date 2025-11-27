using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ProjectBotenReservering.Core.Models
{
    public class WeatherData
    {
        [JsonPropertyName("current_weather")]
        public required CurrentWeather CurrentWeather { get; set; }

        [JsonPropertyName("hourly")]
        public required HourlyData Hourly { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("windspeed")]
        public decimal Windspeed { get; set; }
    }

    public class HourlyData
    {
        [JsonPropertyName("wind_speed_10m")]
        public required List<decimal> WindSpeed10m { get; set; }

        // Als je ook tijden wilt opslaan:
        [JsonPropertyName("time")]
        public required List<string> Time { get; set; }
    }
}
