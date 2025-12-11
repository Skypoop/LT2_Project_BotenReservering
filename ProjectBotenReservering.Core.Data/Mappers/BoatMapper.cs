using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class BoatMapper : IMapper<Boat>
    {
        public Boat Map(IDataReader reader)
        {
            return new Boat(
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetBoolean(reader.GetOrdinal("Steering_Wheel")),
                reader.GetInt32(reader.GetOrdinal("Seats")),
                reader.GetInt32(reader.GetOrdinal("Level")),
                Enum.Parse<BoatType>(reader.GetString(reader.GetOrdinal("Type"))),
                reader.GetInt32(reader.GetOrdinal("Kg")),
                reader.GetBoolean(reader.GetOrdinal("Operational")),
                reader.IsDBNull(reader.GetOrdinal("Club")) ? null : reader.GetString(reader.GetOrdinal("Club")),
                reader.GetInt32(reader.GetOrdinal("Id"))
            );
        }
    }
}