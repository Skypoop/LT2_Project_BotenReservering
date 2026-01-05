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
                reader.GetString(reader.GetOrdinal("Full_Name")),
                reader.GetString(reader.GetOrdinal("Email")),
                reader.GetInt32(reader.GetOrdinal("Scull_level")),
                reader.GetInt32(reader.GetOrdinal("Sweep_level")),
                reader.IsDBNull(reader.GetOrdinal("Club")) ? null : reader.GetString(reader.GetOrdinal("Club")),
                reader.GetBoolean(reader.GetOrdinal("Approved")),
                reader.GetString(reader.GetOrdinal("Password_Hash")),
                reader.GetInt32(reader.GetOrdinal("Id"))
            );
        }
    }
}