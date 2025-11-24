using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class WindConstraintRepository : DatabaseConnection, IWindConstraintRepository
    {
        public WindConstraintRepository()
        {
        }

        public static async Task<WindConstraintRepository> CreateAsync()
        {
            WindConstraintRepository repo = new WindConstraintRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS WindConstraint (
                    Windforce INT NOT NULL PRIMARY KEY,
                    Min_Scull_level INT NOT NULL,
                    Min_Roei_level INT NOT NULL)");

            return repo;
        }

        public async Task<WindConstraint> Add(WindConstraint item)
        {
            string insertQuery = @"INSERT INTO WindConstraint(Windforce, Min_Scull_level, Min_Roei_level) 
                                   VALUES(@Windforce, @MinScullLevel, @MinRoeiLevel)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Windforce", item.Windforce);
                    command.Parameters.AddWithValue("@MinScullLevel", item.MinScullLevel);
                    command.Parameters.AddWithValue("@MinRoeiLevel", item.MinRoeiLevel);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<WindConstraint?> Get(int windforce)
        {
            WindConstraint? constraint = null;
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Roei_level FROM WindConstraint WHERE Windforce = @Windforce";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Windforce", windforce);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            constraint = new WindConstraint(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2)
                            );
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }
            return constraint;
        }

        public async Task<List<WindConstraint>> GetAll()
        {
            var list = new List<WindConstraint>();
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Roei_level FROM WindConstraint";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new(selectQuery, Connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new WindConstraint(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2)
                            ));
                        }
                    }
                }
            } finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }
    }
}