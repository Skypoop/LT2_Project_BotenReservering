using System.Data;

namespace ProjectBotenReservering.Core.Interfaces.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
