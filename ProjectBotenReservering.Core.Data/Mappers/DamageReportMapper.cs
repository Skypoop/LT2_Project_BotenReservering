using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class DamageReportMapper : IMapper<DamageReport>
    {
        public DamageReport Map(IDataReader reader)
        {
            return new DamageReport(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4),
                reader.GetBoolean(5)
            );
        }
    }
}