using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class DamageReportRepository : IDamageReportRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<DamageReport> _mapper;

        public DamageReportRepository(IDbConnectionFactory connectionFactory, IMapper<DamageReport> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public DamageReport Add(DamageReport item)
        {
            string insertQuery = @"INSERT INTO DamageReport(Client_Id, Boat_Id, Damage_Information, Date, Approved) 
                                   VALUES(@ClientId, @BoatId, @DamageInformation, @Date, @Approved);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@ClientId", item.ClientId);
                    command.AddParameter("@BoatId", item.BoatId);
                    command.AddParameter("@DamageInformation", item.DamageInformation);
                    command.AddParameter("@Date", item.Date);
                    command.AddParameter("@Approved", item.Approved);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public DamageReport? Get(int id)
        {
            DamageReport? damageReport = null;
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Id = @Id";

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
                            damageReport = _mapper.Map(reader);
                        }
                    }
                }
            }
            return damageReport;
        }

        public List<DamageReport> GetAll()
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return damageReportList;
        }

        public List<DamageReport> GetByClientId(int clientId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Client_Id = @ClientId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@ClientId", clientId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return damageReportList;
        }

        public List<DamageReport> GetByBoatId(int boatId)
        {
            List<DamageReport> damageReportList = new List<DamageReport>();
            string selectQuery = "SELECT Id, Client_Id, Boat_Id, Damage_Information, Date, Approved FROM DamageReport WHERE Boat_Id = @BoatId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@BoatId", boatId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            damageReportList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return damageReportList;
        }
    }
}