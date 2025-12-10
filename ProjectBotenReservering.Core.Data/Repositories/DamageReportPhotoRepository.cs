using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportPhotoRepository : DatabaseConnection, IDamageReportPhotoRepository
    {
        public DamageReportPhotoRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS DamageReportPhotos (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [DamageReport_Id] INT NOT NULL,
                            [Url] VARCHAR NOT NULL,
                            FOREIGN KEY (DamageReport_Id) REFERENCES DamageReport(Id))");
        }

        public DamageReportPhoto Add(DamageReportPhoto item)
        {
            string insertQuery = @"INSERT INTO DamageReportPhotos(DamageReport_Id, Url) 
                                   VALUES(@DamageReportId, @Url);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@DamageReportId", item.DamageReportId);
                command.Parameters.AddWithValue("@Url", item.Url);
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public DamageReportPhoto? Get(int id)
        {
            DamageReportPhoto? photo = null;
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return photo;
        }

        public List<DamageReportPhoto> GetByDamageReportId(int damageReportId)
        {
            List<DamageReportPhoto> list = new List<DamageReportPhoto>();
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE DamageReport_Id = @DamageReportId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public void Delete(int id)
        {
            string deleteQuery = "DELETE FROM DamageReportPhotos WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}

