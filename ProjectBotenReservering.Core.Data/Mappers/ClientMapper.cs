using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ClientMapper : IMapper<Client>
    {
        public Client Map(IDataReader reader)
        {
            return new Client(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.GetBoolean(6),
                reader.GetString(7),
                reader.GetInt32(0)
            );
        }
    }
}