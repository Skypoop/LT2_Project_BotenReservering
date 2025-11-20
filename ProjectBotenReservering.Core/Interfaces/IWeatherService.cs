namespace ProjectBotenReservering.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<int> GetWeatherAsync();
    }
}
