using System.Data;

namespace ProjectBotenReservering.Core.Interfaces.Database
{
    public interface IDatabaseSeeder
    {
        int Order { get; }
        void Seed(IDbConnection connection);
    }
}
