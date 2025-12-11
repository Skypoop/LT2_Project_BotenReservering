using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ManagementTaskMapper : IMapper<ManagementTask>
    {
        public ManagementTask Map(IDataReader reader)
        {
            return new ManagementTask(reader.GetInt32(0), reader.GetString(1));
        }
    }
}