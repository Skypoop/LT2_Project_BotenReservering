using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ReservationMapper : IMapper<Reservation>
    {
        public Reservation Map(IDataReader reader)
        {
            return new Reservation(
                reader.GetDateTime(1),
                reader.GetDateTime(2),
                reader.GetDateTime(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                reader.GetBoolean(6),
                reader.GetInt32(0),
                reader.GetBoolean(7)
            );
        }
    }
}