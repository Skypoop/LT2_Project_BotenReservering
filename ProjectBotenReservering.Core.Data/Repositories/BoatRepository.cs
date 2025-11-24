using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class BoatRepository : DatabaseConnection, IBoatRepository
    {
        public BoatRepository()
        {
        }

        public static async Task<BoatRepository> CreateAsync()
        {
            BoatRepository repo = new BoatRepository();

            await repo.CreateTableAsync(@"CREATE TABLE IF NOT EXISTS Boat (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] STRING NOT NULL,
                            [Steering_Wheel] BOOLEAN NOT NULL,
                            [Seats] INT NOT NULL,
                            [Level] INT NOT NULL,
                            [Type] CHAR NOT NULL,
                            [Kg] INT NOT NULL,
                            [Operational] BOOLEAN NOT NULL,
                            [Club] VARCHAR)");

            List<Boat> boats = await repo.GetAll();
            bool anyBoatExists = boats.Count > 0;

            if (!anyBoatExists)
            {
                await repo.Add(new Boat("Skiff van Kunststof", false, 2, 1, BoatType.S, 45, true, "Local Club"));
                await repo.Add(new Boat("Dubbel Twee van Kunststof", false, 2, 1, BoatType.S, 46, true, "Local Club"));
                await repo.Add(new Boat("Twee zonder van Kunststof", false, 2, 3, BoatType.B, 46, true, "Local Club"));
                await repo.Add(new Boat("Twee met van Kunststof", true, 3, 3, BoatType.B, 46, true, "Local Club"));
                await repo.Add(new Boat("Dubbel vier van Kunststof", false, 4, 3, BoatType.S, 50, true, "Local Club"));
                await repo.Add(new Boat("Dubbel vier met van Kunststof", true, 5, 3, BoatType.B, 52, true, "Local Club"));
                await repo.Add(new Boat("Vier zonder van Kunststof", false, 4, 3, BoatType.B, 50, true, "Local Club"));
                await repo.Add(new Boat("Vier met van Kunststof", true, 5, 3, BoatType.B, 52, true, "Local Club"));
                await repo.Add(new Boat("Acht van Kunststof", true, 9, 3, BoatType.B, 55, true, "Local Club"));
            }

            return repo;
        }

        public async Task<Boat> Add(Boat item)
        {
            const string insertQuery = @"INSERT INTO Boat(Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club) 
                                         VALUES(@Name, @SteeringWheel, @Seats, @Level, @Type, @Kg, @Operational, @Club);
                                         SELECT last_insert_rowid();";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(insertQuery, Connection);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@SteeringWheel", item.SteeringWheel);
                command.Parameters.AddWithValue("@Seats", item.Seats);
                command.Parameters.AddWithValue("@Level", item.Level);
                command.Parameters.AddWithValue("@Type", item.Type.ToString());
                command.Parameters.AddWithValue("@Kg", item.Kg);
                command.Parameters.AddWithValue("@Operational", item.Operational);
                command.Parameters.AddWithValue("@Club", item.Club ?? (object)DBNull.Value);

                object result = await command.ExecuteScalarAsync();
                item.Id = Convert.ToInt32(result);
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return item;
        }

        public async Task<Boat?> Get(int id)
        {
            Boat? boat = null;
            const string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(selectQuery, Connection);
                command.Parameters.AddWithValue("@Id", id);
                await using SqliteDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    boat = MapReaderToBoat(reader);
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return boat;
        }

        public async Task Delete(int boatId)
        {
            const string deleteQuery = "DELETE FROM Boat WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(deleteQuery, Connection);
                command.Parameters.AddWithValue("@Id", boatId);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task DeleteAll()
        {
            const string deleteQuery = "DELETE FROM Boat";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(deleteQuery, Connection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<List<Boat>> GetAll()
        {
            List<Boat> boatList = new List<Boat>();
            const string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(selectQuery, Connection);
                await using SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    boatList.Add(MapReaderToBoat(reader));
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return boatList;
        }

        public async Task<List<Boat>> GetOperationalBoats()
        {
            List<Boat> boatList = new List<Boat>();
            const string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Operational = 1";

            await OpenConnectionAsync();

            try
            {
                await using SqliteCommand command = new SqliteCommand(selectQuery, Connection);
                await using SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    boatList.Add(MapReaderToBoat(reader));
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

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
