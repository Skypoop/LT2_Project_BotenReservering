using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportRepository : DatabaseConnection, IDamageReportRepository
    {
        public DamageReportRepository()
        {
        }

        public static async Task<DamageReportRepository> CreateAsync()
        {
            DamageReportRepository repo = new DamageReportRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS DamageReport (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Client_Id] INT NOT NULL,
                    [Boat_Id] INT NOT NULL,
                    [Damage_Information] LONGVARCHAR NOT NULL,
                    [Date] DATETIME NOT NULL,
                    [Approved] BOOLEAN NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))");

            return repo;
        }

        public async Task<DamageReport> Add(DamageReport item)
        {
            string insertQuery = @"INSERT INTO DamageReport(Client_Id, Boat_Id, Damage_Information, Date, Approved) 
                                   VALUES(@ClientId, @BoatId, @DamageInformation, @Date, @Approved);
                                   SELECT last_insert_rowid();";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", item.ClientId);
                    command.Parameters.AddWithValue("@BoatId", item.BoatId);
                    command.Parameters.AddWithValue("@DamageInformation", item.DamageInformation);
                    command.Parameters.AddWithValue("@Date", item.Date);
                    command.Parameters.AddWithValue("@Approved", item.Approved);

                    object? result = command.ExecuteScalar();
                    if (result != null)
                    {
                        item.Id = Convert.ToInt32(result);
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<DamageReport?> Get(int id)
        {
            DamageReport? damageReport = null;
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            damageReport = MapReaderToDamageReport(reader);
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return damageReport;
        }

        public async Task<List<DamageReport>> GetAll()
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(MapReaderToDamageReport(reader));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return damageReportList;
        }

        public async Task<List<DamageReport>> GetByClientId(int clientId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Client_Id = @ClientId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(MapReaderToDamageReport(reader));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return damageReportList;
        }

        public async Task<List<DamageReport>> GetByBoatId(int boatId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Boat_Id = @BoatId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@BoatId", boatId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(MapReaderToDamageReport(reader));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return damageReportList;
        }

        private DamageReport MapReaderToDamageReport(SqliteDataReader reader)
        {
            return new DamageReport(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetString(3),
                reader.GetDateTime(4),
                reader.GetBoolean(5)
            );
        }
    }
}
