using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class RoleManagementTaskMapper : IMapper<RoleManagementTask>
    {
        public RoleManagementTask Map(IDataReader reader)
        {
            return new RoleManagementTask(reader.GetString(0), reader.GetInt32(1));
        }
    }
}