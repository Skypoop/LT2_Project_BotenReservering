using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportPhotoRepository : IDamageReportPhotoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<DamageReportPhoto> _mapper;

        public DamageReportPhotoRepository(IDbConnectionFactory connectionFactory, IMapper<DamageReportPhoto> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public DamageReportPhoto Add(DamageReportPhoto item)
        {
            string insertQuery = @"INSERT INTO DamageReportPhotos(DamageReport_Id, Url) 
                                   VALUES(@DamageReportId, @Url);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@DamageReportId", item.DamageReportId);
                    command.AddParameter("@Url", item.Url);
                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public DamageReportPhoto? Get(int id)
        {
            DamageReportPhoto? photo = null;
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE Id = @Id";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@Id", id);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            photo = _mapper.Map(reader);
                        }
                    }
                }
            }
            return photo;
        }

        public List<DamageReportPhoto> GetByDamageReportId(int damageReportId)
        {
            List<DamageReportPhoto> list = new List<DamageReportPhoto>();
            string selectQuery = "SELECT Id, DamageReport_Id, Url FROM DamageReportPhotos WHERE DamageReport_Id = @DamageReportId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@DamageReportId", damageReportId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(int id)
        {
            string deleteQuery = "DELETE FROM DamageReportPhotos WHERE Id = @Id";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}