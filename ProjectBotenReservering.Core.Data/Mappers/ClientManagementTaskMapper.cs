using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Mappers
{
    public class ClientManagementTaskMapper : IMapper<ClientManagementTask>
    {
        public ClientManagementTask Map(IDataReader reader)
        {
            return new ClientManagementTask(reader.GetInt32(0), reader.GetInt32(1));
        }
    }
}