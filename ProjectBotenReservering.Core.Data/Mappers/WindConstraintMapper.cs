using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class WindConstraintMapper : IMapper<WindConstraint>
    {
        public WindConstraint Map(IDataReader reader)
        {
            return new WindConstraint(
                reader.GetInt32(reader.GetOrdinal("Windforce")),
                reader.GetInt32(reader.GetOrdinal("Min_Scull_level")),
                reader.GetInt32(reader.GetOrdinal("Min_Sweep_level"))
            );
        }
    }
}