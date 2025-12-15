using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class CompetitionMapper : IMapper<Competition>
    {
        public Competition Map(IDataReader reader)
        {
            return new Competition
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StartDateTime = reader.GetDateTime(reader.GetOrdinal("Start_DateTime")),
                EndDateTime = reader.GetDateTime(reader.GetOrdinal("End_DateTime")),
                CompetitionName = reader.GetString(reader.GetOrdinal("Competition_Name"))
            };
        }
    }
}