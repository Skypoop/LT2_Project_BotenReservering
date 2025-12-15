using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ClientRoleMapper : IMapper<ClientRole>
    {
        public ClientRole Map(IDataReader reader)
        {
            return new ClientRole(
                reader.GetString(reader.GetOrdinal("Role_Name")),
                reader.GetInt32(reader.GetOrdinal("Client_Id"))
            );
        }
    }
}