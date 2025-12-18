using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Helpers
{
    public static class MessageHelper
    {
        public static string ConvertWeatherAuthorizationMessageToUi(WeatherAuthorizationResultEnum weatherAuthorizationResult)
        {
            return weatherAuthorizationResult switch
            {
                WeatherAuthorizationResultEnum.RequiresHigherBoatLevel => "LET OP: Voor deze datum en tijd is het weer heftig voor een of meerdere geselecteerde boten!",
                WeatherAuthorizationResultEnum.DateTooFarInFuture => "LET OP: Het weer kan alleen voorspeld worden tot 7 dagen vooruit!",
                WeatherAuthorizationResultEnum.DataNotLoaded => "LET OP: Kon geen weer data inladen",
                _ => string.Empty,
            };
        }
    }
}
