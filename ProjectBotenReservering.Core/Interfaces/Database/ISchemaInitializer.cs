using System.Data;

namespace ProjectBotenReservering.Core.Interfaces.Database
{
    public interface ISchemaInitializer
    {
        void Initialize(IDbConnection connection);
    }
}
