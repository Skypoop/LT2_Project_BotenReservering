using System.Data;

namespace ProjectBotenReservering.Core.Interfaces.Database
{
    public interface IDatabaseFixture
    {
        int Order { get; }
        void Seed(IDbConnection connection);
    }
}