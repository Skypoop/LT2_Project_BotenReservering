using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class DamageReportPhotoMapper : IMapper<DamageReportPhoto>
    {
        public DamageReportPhoto Map(IDataReader reader)
        {
            return new DamageReportPhoto(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2)
            );
        }
    }
}