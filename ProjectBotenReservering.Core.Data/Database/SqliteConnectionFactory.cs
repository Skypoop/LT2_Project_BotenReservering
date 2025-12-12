using System.Data;
using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Interfaces;
using ProjectBotenReservering.Core.Interfaces.Database;

namespace ProjectBotenReservering.Core.Data.Database
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}