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
                reader.GetDateTime(reader.GetOrdinal("Created_At")),
                reader.GetDateTime(reader.GetOrdinal("Start_Time")),
                reader.GetDateTime(reader.GetOrdinal("End_Time")),
                reader.GetInt32(reader.GetOrdinal("Client_Id")),
                reader.GetInt32(reader.GetOrdinal("Boat_Id")),
                reader.GetBoolean(reader.GetOrdinal("Approved")),
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetBoolean(reader.GetOrdinal("Active"))
            );
        }
    }
}