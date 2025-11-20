using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class BoatRepository : DatabaseConnection, IBoatRepository
    {
        public BoatRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Boat (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] STRING NOT NULL,
                            [Steering_Wheel] BOOLEAN NOT NULL,
                            [Seats] INT NOT NULL,
                            [Level] INT NOT NULL,
                            [Type] CHAR NOT NULL,
                            [Kg] INT NOT NULL,
                            [Operational] BOOLEAN NOT NULL,
                            [Club] VARCHAR)");

            List<Boat> boats = GetAll();
            bool anyBoatExists = boats.Count > 0;
            
            if (anyBoatExists == false)
            {
                // Add boats to database if none exist
                Add(new Boat("Skiff van Kunststof", false, 2, 1, BoatType.S, 45, true,"Local Club"));
                Add(new Boat("Dubbel Twee van Kunststof", false, 2, 1, BoatType.S, 46, true, "Local Club"));
                Add(new Boat("Twee zonder van Kunststof", false, 2, 3, BoatType.B, 46, true,"Local Club"));
                Add(new Boat("Twee met van Kunststof", true, 3, 3, BoatType.B, 46, true,"Local Club"));
                Add(new Boat("Dubbel vier van Kunststof", false, 4, 3, BoatType.S, 50, true,"Local Club"));
                Add(new Boat("Dubbel vier met van Kunststof", true, 5, 3, BoatType.B, 52, true,"Local Club"));
                Add(new Boat("Vier zonder van Kunststof", false, 4, 3, BoatType.B, 50, true,"Local Club"));
                Add(new Boat("Vier met van Kunststof", true, 5, 3, BoatType.B, 52, true,"Local Club"));
                Add(new Boat("Acht van Kunststof", true, 9, 3, BoatType.B, 55, true,"Local Club"));
            }
        }

        public Boat Add(Boat item)
        {
            string insertQuery = @"INSERT INTO Boat(Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club) 
                                   VALUES(@Name, @SteeringWheel, @Seats, @Level, @Type, @Kg, @Operational, @Club);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@SteeringWheel", item.SteeringWheel);
                command.Parameters.AddWithValue("@Seats", item.Seats);
                command.Parameters.AddWithValue("@Level", item.Level);
                command.Parameters.AddWithValue("@Type", item.Type.ToString());
                command.Parameters.AddWithValue("@Kg", item.Kg);
                command.Parameters.AddWithValue("@Operational", item.Operational);
                command.Parameters.AddWithValue("@Club", item.Club ?? (object)DBNull.Value);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Boat? Get(int id)
        {
            Boat? boat = null;
            string selectQuery = "SELECT Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club, Id FROM Boat WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        boat = MapReaderToBoat(reader);
                    }
                }
            }

            CloseConnection();
            return boat;
        }
        
        public void Delete(int boatId)
        {
            string deleteQuery = "DELETE FROM Boat WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", boatId);
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }
        
        public void DeleteAll()
        {
            string deleteQuery = "DELETE FROM Boat";
            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }
        
        public List<Boat> GetAll()
        {
            var boatList = new List<Boat>();
            string selectQuery = "SELECT * FROM Boat";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        boatList.Add(MapReaderToBoat(reader));
                    }
                }
            }

            CloseConnection();
            return boatList;
        }

        public List<Boat> GetOperationalBoats()
        {
            var boatList = new List<Boat>();
            string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Operational = 1";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        boatList.Add(MapReaderToBoat(reader));
                    }
                }
            }

            CloseConnection();
            return boatList;
        }

        private Boat MapReaderToBoat(SqliteDataReader reader)
        {
            return new Boat(
                reader.GetString(1),
                reader.GetBoolean(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                Enum.Parse<BoatType>(reader.GetString(5)),
                reader.GetInt32(6),
                reader.GetBoolean(7),
                reader.IsDBNull(8) ? null : reader.GetString(8),
                reader.GetInt32(0)
            );
        }
    }
}

