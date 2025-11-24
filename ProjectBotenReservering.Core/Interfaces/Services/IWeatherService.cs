namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<int> GetWeatherAsync();
    }
}
