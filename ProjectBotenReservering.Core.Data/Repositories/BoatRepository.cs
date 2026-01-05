using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class BoatRepository : IBoatRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Boat> _mapper;

        public BoatRepository(IDbConnectionFactory connectionFactory, IMapper<Boat> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Boat Add(Boat item)
        {
            string insertQuery = @"INSERT INTO Boat(Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club) 
                                   VALUES(@Name, @SteeringWheel, @Seats, @Level, @Type, @Kg, @Operational, @Club);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@Name", item.Name);
                    command.AddParameter("@SteeringWheel", item.SteeringWheel);
                    command.AddParameter("@Seats", item.Seats);
                    command.AddParameter("@Level", item.Level);
                    command.AddParameter("@Type", item.Type.ToString());
                    command.AddParameter("@Kg", item.Kg);
                    command.AddParameter("@Operational", item.Operational);
                    command.AddParameter("@Club", item.Club ?? (object)DBNull.Value);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public Boat? Get(int id)
        {
            Boat? boat = null;
            string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Id = @Id";

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
                            boat = _mapper.Map(reader);
                        }
                    }
                }
            }
            return boat;
        }

        public void Delete(int boatId)
        {
            string deleteQuery = "DELETE FROM Boat WHERE Id = @Id";
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@Id", boatId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAll()
        {
            string deleteQuery = "DELETE FROM Boat";
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Boat> GetAll()
        {
            List<Boat> boatList = new List<Boat>();
            string selectQuery = "SELECT * FROM Boat";

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
                            boatList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return boatList;
        }

        public List<Boat> GetAllFromName(string boatName)
        {
            List<Boat> boatlist = new List<Boat>();
            string selecQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Name = @BoatName";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selecQuery;
                    command.AddParameter("@BoatName", boatName);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            boatlist.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return boatlist;
        }


        public List<Boat> GetOperationalBoats()
        {
            List<Boat> boatList = new List<Boat>();
            string selectQuery = "SELECT Id, Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club FROM Boat WHERE Operational = 1";

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
                            boatList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return boatList;
        }
    }
}