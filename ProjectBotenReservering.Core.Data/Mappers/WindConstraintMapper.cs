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
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2)
            );
        }
    }
}