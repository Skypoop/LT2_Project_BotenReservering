using System.Text.Json.Serialization;

namespace ProjectBotenReservering.Core.Models
{
    public class CurrentWeather
    {
        [JsonPropertyName("windspeed")]
        public decimal Windspeed { get; set; }
    }
}
