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
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetInt32(reader.GetOrdinal("Client_Id")),
                reader.GetInt32(reader.GetOrdinal("Boat_Id")),
                reader.GetString(reader.GetOrdinal("Damage_Information")),
                reader.GetDateTime(reader.GetOrdinal("Date")),
                reader.GetBoolean(reader.GetOrdinal("Approved"))
            );
        }
    }
}