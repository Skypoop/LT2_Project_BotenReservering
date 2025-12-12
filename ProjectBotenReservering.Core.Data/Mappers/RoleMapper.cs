using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class RoleMapper : IMapper<Role>
    {
        public Role Map(IDataReader reader)
        {
            return new Role(reader.GetString(reader.GetOrdinal("Name")));
        }
    }
}