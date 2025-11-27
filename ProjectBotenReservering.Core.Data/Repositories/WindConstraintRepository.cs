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
                            [Min_Sweep_level] INT NOT NULL)");

            List<WindConstraint> windConstraints = GetAll();
            bool anyWindConstraintsExist = windConstraints.Count > 0;

            if (anyWindConstraintsExist == false)
            {
                Add(new WindConstraint(1, 10, 10));
                Add(new WindConstraint(2, 10, 10));
                Add(new WindConstraint(3, 10, 10));
                Add(new WindConstraint(4, 10, 10));
                Add(new WindConstraint(5, 10, 10));
                Add(new WindConstraint(6, 10, 10));
                Add(new WindConstraint(7, 10, 10));
                Add(new WindConstraint(8, 10, 10));
                Add(new WindConstraint(9, 10, 10));
                Add(new WindConstraint(10, 10, 10));
                Add(new WindConstraint(11, 10, 10));
                Add(new WindConstraint(12, 10, 10));
            }
        }

        public WindConstraint Add(WindConstraint item)
        {
            string insertQuery = @"INSERT INTO WindConstraint(Windforce, Min_Scull_level, Min_Sweep_level) 
                                   VALUES(@Windforce, @MinScullLevel, @MinSweepLevel)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Windforce", item.Windforce);
                command.Parameters.AddWithValue("@MinScullLevel", item.MinScullLevel);
                command.Parameters.AddWithValue("@MinSweepLevel", item.MinSweepLevel);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public WindConstraint? Get(int windforce)
        {
            WindConstraint? constraint = null;
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Sweep_level FROM WindConstraint WHERE Windforce = @Windforce";
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

        public List<WindConstraint> GetAll()
        {
            var list = new List<WindConstraint>();
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Sweep_level FROM WindConstraint";
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

