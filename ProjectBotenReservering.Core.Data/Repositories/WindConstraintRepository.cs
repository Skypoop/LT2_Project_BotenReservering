using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class WindConstraintRepository : DatabaseConnection, IWindConstraintRepository
    {
        public WindConstraintRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS WindConstraint (
                            [Windforce] INT NOT NULL PRIMARY KEY,
                            [Min_Scull_level] INT NOT NULL,
                            [Min_Roei_level] INT NOT NULL)");
        }

        public async Task<WindConstraint> Add(WindConstraint item)
        {
            string insertQuery = @"INSERT INTO WindConstraint(Windforce, Min_Scull_level, Min_Roei_level) 
                                   VALUES(@Windforce, @MinScullLevel, @MinRoeiLevel)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Windforce", item.Windforce);
                command.Parameters.AddWithValue("@MinScullLevel", item.MinScullLevel);
                command.Parameters.AddWithValue("@MinRoeiLevel", item.MinRoeiLevel);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public async Task<WindConstraint?> Get(int windforce)
        {
            WindConstraint? constraint = null;
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Roei_level FROM WindConstraint WHERE Windforce = @Windforce";
            OpenConnection();

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

            CloseConnection();
            return constraint;
        }

        public async Task<List<WindConstraint>> GetAll()
        {
            var list = new List<WindConstraint>();
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Roei_level FROM WindConstraint";
            OpenConnection();

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

            CloseConnection();
            return list;
        }
    }
}

