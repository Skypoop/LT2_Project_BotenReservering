using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;    
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Reservation> _mapper;

        public ReservationRepository(IDbConnectionFactory connectionFactory, IMapper<Reservation> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Reservation Add(Reservation item)
        {
            string insertQuery = @"INSERT INTO Reservation(Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active) 
                                   VALUES(@CreatedAt, @StartTime, @EndTime, @ClientId, @BoatId, @Approved, @Active);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@CreatedAt", item.CreatedAt);
                    command.AddParameter("@StartTime", item.StartTime);
                    command.AddParameter("@EndTime", item.EndTime);
                    command.AddParameter("@ClientId", item.ClientId);
                    command.AddParameter("@BoatId", item.BoatId);
                    command.AddParameter("@Approved", item.Approved);
                    command.AddParameter("@Active", item.Active);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public Reservation? Get(int id)
        {
            Reservation? reservation = null;
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active FROM Reservation WHERE Id = @Id";

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
                            reservation = _mapper.Map(reader);
                        }
                    }
                }
            }
            return reservation;
        }

        public List<Reservation> GetAll()
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active FROM Reservation";

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
                            reservationList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return reservationList;
        }

        public List<Reservation> GetByClientId(int clientId)
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active FROM Reservation WHERE Client_Id = @ClientId";

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
                            reservationList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return reservationList;
        }

        public List<Reservation> GetByBoatId(int boatId)
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active FROM Reservation WHERE Boat_Id = @BoatId";

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
                            reservationList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return reservationList;
        }

        public void CancelReservationsByIds(List<int> reservationIds)
        {
            if (reservationIds == null || reservationIds.Count == 0)
                return;

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bouw de IN clause met parameters
                        string parameters = string.Join(",",
                            reservationIds.Select((_, i) => $"@Id{i}"));

                        string updateQuery = $"UPDATE Reservation SET Active = 0 WHERE Id IN ({parameters})";

                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = updateQuery;

                            // Voeg alle parameters toe
                            for (int i = 0; i < reservationIds.Count; i++)
                            {
                                command.AddParameter($"@Id{i}", reservationIds[i]);
                            }

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
