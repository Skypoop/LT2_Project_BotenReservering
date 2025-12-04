using ProjectBotenReservering.Core.Interfaces.Services;

namespace ProjectBotenReservering.Core.Services
{
    public class WindforceService : IWindforceService
    {
        public static int GetWindforce(decimal windspeedKmh)
        {
            decimal[] limits = { 1, 5, 11, 19, 28, 38, 49, 61, 74, 88, 102, 177 };  // Upper wind-speed thresholds (km/h) for Beaufort scale levels 0–12

            for (int force = 0; force < limits.Length; force++)
            {
                if (windspeedKmh <= limits[force])
                    return force;
            }

            return 12;

        }
            
    }
}
