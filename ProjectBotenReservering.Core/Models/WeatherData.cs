using System.Text.Json.Serialization;

namespace ProjectBotenReservering.Core.Models
{
    public class WeatherData
    {
        [JsonPropertyName("current_weather")]
        public required CurrentWeather CurrentWeather { get; set; }
    }

    public class CurrentWeather
    {
        [JsonPropertyName("windspeed")]
        public decimal Windspeed { get; set; }
    }
}
