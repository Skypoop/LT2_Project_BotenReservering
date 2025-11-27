using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<int> GetWeatherAsync(DateTime? beginDate = null, DateTime? endDate = null);
    }
}
