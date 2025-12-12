using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ClientReservationMapper : IMapper<ClientReservation>
    {
        public ClientReservation Map(IDataReader reader)
        {
            return new ClientReservation(
                reader.GetInt32(reader.GetOrdinal("Client_Id")),
                reader.GetInt32(reader.GetOrdinal("Reservation_Id"))
            );
        }
    }
}