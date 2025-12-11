using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ReservationMatchMapper : IMapper<ReservationMatch>
    {
        public ReservationMatch Map(IDataReader reader)
        {
            return new ReservationMatch(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));
        }
    }
}