using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportPhotoRepository : DatabaseConnection, IDamageReportPhotoRepository
    {
        public DamageReportPhotoRepository()
        {
        }

        public static async Task<DamageReportPhotoRepository> CreateAsync()
        {
            DamageReportPhotoRepository repo = new DamageReportPhotoRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS DamageReportPhotos (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [DamageReport_Id] INT NOT NULL,
                    [Url] VARCHAR NOT NULL,
                    FOREIGN KEY (DamageReport_Id) REFERENCES DamageReport(Id))");

            return repo;
        }

        public async Task<DamageReportPhoto> Add(DamageReportPhoto item)
        {
            string insertQuery = @"INSERT INTO DamageReportPhotos(DamageReport_Id, Url) 
                                   VALUES(@DamageReportId, @Url);
                                   SELECT last_insert_rowid();";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@DamageReportId", item.DamageReportId);
                    command.Parameters.AddWithValue("@Url", item.Url);

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

        public async Task<DamageReportPhoto?> Get(int id)
        {
            DamageReportPhoto? photo = null;
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE Id = @Id";

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
                            photo = new DamageReportPhoto(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetString(2)
                            );
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return photo;
        }

        public async Task<List<DamageReportPhoto>> GetByDamageReportId(int damageReportId)
        {
            List<DamageReportPhoto> list = new List<DamageReportPhoto>();
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE DamageReport_Id = @DamageReportId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@DamageReportId", damageReportId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DamageReportPhoto(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetString(2)
                            ));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }

        public async Task Delete(int id)
        {
            string deleteQuery = "DELETE FROM DamageReportPhotos WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(deleteQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }
        }
    }
}
