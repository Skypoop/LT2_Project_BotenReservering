using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class MatchMapper : IMapper<Match>
    {
        public Match Map(IDataReader reader)
        {
            return new Match
            {
                Id = reader.GetInt32(0),
                StartDateTime = reader.GetDateTime(1),
                EndDateTime = reader.GetDateTime(2),
                MatchName = reader.GetString(3)
            };
        }
    }
}