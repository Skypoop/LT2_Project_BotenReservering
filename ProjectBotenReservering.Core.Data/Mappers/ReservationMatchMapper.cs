using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ReservationMatchMapper : IMapper<ReservationMatch>
    {
        public ReservationMatch Map(IDataReader reader)
        {
            return new ReservationMatch(
                reader.GetInt32(reader.GetOrdinal("Match_Id")),
                reader.GetInt32(reader.GetOrdinal("Reservation_Id")),
                reader.GetString(reader.GetOrdinal("Team_Name"))
            );
        }
    }
}