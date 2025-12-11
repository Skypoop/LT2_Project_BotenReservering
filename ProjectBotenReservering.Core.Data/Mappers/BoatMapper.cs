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
                reader.GetString(1),
                reader.GetBoolean(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                Enum.Parse<BoatType>(reader.GetString(5)),
                reader.GetInt32(6),
                reader.GetBoolean(7),
                reader.IsDBNull(8) ? null : reader.GetString(8),
                reader.GetInt32(0)
            );
        }
    }
}