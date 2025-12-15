using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class WindConstraintRepository : IWindConstraintRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<WindConstraint> _mapper;

        public WindConstraintRepository(IDbConnectionFactory connectionFactory, IMapper<WindConstraint> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public WindConstraint Add(WindConstraint item)
        {
            string insertQuery = @"INSERT INTO WindConstraint(Windforce, Min_Scull_level, Min_Sweep_level) 
                                   VALUES(@Windforce, @MinScullLevel, @MinSweepLevel)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@Windforce", item.Windforce);
                    command.AddParameter("@MinScullLevel", item.MinScullLevel);
                    command.AddParameter("@MinSweepLevel", item.MinSweepLevel);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public WindConstraint? Get(int windforce)
        {
            WindConstraint? constraint = null;
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Sweep_level FROM WindConstraint WHERE Windforce = @Windforce";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@Windforce", windforce);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            constraint = _mapper.Map(reader);
                        }
                    }
                }
            }
            return constraint;
        }

        public List<WindConstraint> GetAll()
        {
            List<WindConstraint> list = new List<WindConstraint>();
            string selectQuery = "SELECT Windforce, Min_Scull_level, Min_Sweep_level FROM WindConstraint";

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
                            list.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return list;
        }
    }
}