using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportRepository : DatabaseConnection, IDamageReportRepository
    {
        public DamageReportRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS DamageReport (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Client_Id] INT NOT NULL,
                            [Boat_Id] INT NOT NULL,
                            [Damage_Information] LONGVARCHAR NOT NULL,
                            [Date] DATETIME NOT NULL,
                            [Approved] BOOLEAN NOT NULL,
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                            FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))");
        }

        public DamageReport Add(DamageReport item)
        {
            string insertQuery = @"INSERT INTO DamageReport(Client_Id, Boat_Id, Damage_Information, Date, Approved) 
                                   VALUES(@ClientId, @BoatId, @DamageInformation, @Date, @Approved);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", item.ClientId);
                command.Parameters.AddWithValue("@BoatId", item.BoatId);
                command.Parameters.AddWithValue("@DamageInformation", item.DamageInformation);
                command.Parameters.AddWithValue("@Date", item.Date);
                command.Parameters.AddWithValue("@Approved", item.Approved);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public DamageReport? Get(int id)
        {
            DamageReport? damageReport = null;
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return damageReport;
        }

        public List<DamageReport> GetAll()
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        damageReportList.Add(MapReaderToDamageReport(reader));
                    }
                }
            }

            CloseConnection();
            return damageReportList;
        }

        public List<DamageReport> GetByClientId(int clientId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return damageReportList;
        }

        public List<DamageReport> GetByBoatId(int boatId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Boat_Id = @BoatId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
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

