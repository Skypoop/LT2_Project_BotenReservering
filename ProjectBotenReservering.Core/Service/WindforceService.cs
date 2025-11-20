namespace ProjectBotenReservering.Core.Service
{
    public class WindforceService
    {
        public static int GetWindforce(decimal windspeedKmh)
        {
            decimal[] limits = { 1, 5, 11, 19, 28, 38, 49, 61, 74, 88, 102, 177 };  //in km/h windkracht 0 -> 12

            for (int force = 0; force < limits.Length; force++)
            {
                if (windspeedKmh <= limits[force])
                    return force;
            }

            return 12;

        }
            
    }
}
