using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ReservationCompetitionMapper : IMapper<ReservationCompetition>
    {
        public ReservationCompetition Map(IDataReader reader)
        {
            return new ReservationCompetition(
                reader.GetInt32(reader.GetOrdinal("Competition_Id")),
                reader.GetInt32(reader.GetOrdinal("Reservation_Id")),
                reader.GetString(reader.GetOrdinal("Team_Name"))
            );
        }
    }
}