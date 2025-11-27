using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services
{
    public interface IWeatherRuleAuthencation
    {
        Task<bool> IsAllowedToSail(List<Client> clients, int boatId, DateTime beginDate, DateTime endDate);
    }
}
