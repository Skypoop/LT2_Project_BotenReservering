using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Services
{
    public class WeatherRuleAuthencation : IWeatherRuleAuthencation
    {
        private readonly IWeatherService _weatherService;
        private readonly IBoatService _boatService;
        private readonly IWindConstraintRepository _windConstraintRepository;


        public WeatherRuleAuthencation(IWeatherService weatherService, IBoatService boatService, IWindConstraintRepository windConstraintRepository)
        {
            _weatherService = weatherService;
            _boatService = boatService;
            _windConstraintRepository = windConstraintRepository;
        }

        public async Task<bool> IsAllowedToSail(List<Client> clients, int boatId, DateTime beginDate, DateTime endDate)
        {
            int windforce = await _weatherService.GetWeatherAsync(beginDate, endDate);
            WindConstraint? maxLevels = _windConstraintRepository.Get(windforce);
            Boat? boat = _boatService.Get(boatId);

            if (maxLevels == null || boat == null)
            {
                return false;
            }

            if ((boat.Type == BoatType.B && boat.Level > maxLevels.MinSweepLevel) || (boat.Type == BoatType.S && boat.Level > maxLevels.MinScullLevel)) {
                return false;
            }

            foreach (var client in clients)
            {
                if (boat.Type == BoatType.S && client.ScullLevel > maxLevels.MinScullLevel)
                {
                    return false;
                }

                if (boat.Type == BoatType.B && client.SweepLevel > maxLevels.MinSweepLevel)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
