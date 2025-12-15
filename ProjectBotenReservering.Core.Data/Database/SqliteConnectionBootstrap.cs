using System.Collections.Generic;
using System.Data;
using System.Linq;
using ProjectBotenReservering.Core.Interfaces.Database;

namespace ProjectBotenReservering.Core.Data.Database
{
    public class SqliteDatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ISchemaInitializer _schemaInitializer;
        private readonly IEnumerable<IDatabaseSeeder> _seeders;
        private readonly IEnumerable<IDatabaseFixture> _fixtures;

        public SqliteDatabaseBootstrap(
            IDbConnectionFactory connectionFactory,
            ISchemaInitializer schemaInitializer,
            IEnumerable<IDatabaseSeeder> seeders,
            IEnumerable<IDatabaseFixture> fixtures)
        {
            _connectionFactory = connectionFactory;
            _schemaInitializer = schemaInitializer;
            _seeders = seeders;
            _fixtures = fixtures;
        }

        public void Setup()
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                _schemaInitializer.Initialize(connection);

                List<IDatabaseSeeder> orderedSeeders = _seeders.OrderBy(x => x.Order).ToList();
                foreach (IDatabaseSeeder seeder in orderedSeeders)
                {
                    seeder.Seed(connection);
                }

                List<IDatabaseFixture> orderedFixtures = _fixtures.OrderBy(x => x.Order).ToList();
                foreach (IDatabaseFixture fixture in orderedFixtures)
                {
                    fixture.Seed(connection);
                }
            }
        }
    }
}