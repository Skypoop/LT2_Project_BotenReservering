using System.Text.Json.Serialization;

namespace ProjectBotenReservering.Core.Models
{
    public class HourlyData
    {
        [JsonPropertyName("wind_speed_10m")]
        public required List<decimal> WindSpeed10m { get; set; }

        [JsonPropertyName("time")]
        public required List<string> Time { get; set; }
    }
}
