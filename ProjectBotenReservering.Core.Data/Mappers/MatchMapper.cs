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
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StartDateTime = reader.GetDateTime(reader.GetOrdinal("Start_DateTime")),
                EndDateTime = reader.GetDateTime(reader.GetOrdinal("End_DateTime")),
                MatchName = reader.GetString(reader.GetOrdinal("Match_Name"))
            };
        }
    }
}