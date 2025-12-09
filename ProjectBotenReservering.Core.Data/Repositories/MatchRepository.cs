using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class MatchRepository : DatabaseConnection, IMatchRepository
    {
        public MatchRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Match (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Start_DateTime] DATETIME NOT NULL,
                            [End_DateTime] DATETIME NOT NULL,
                            [Match_Name] VARCHAR NOT NULL)");
        }

        public Match Add(Match item)
        {
            string insertQuery = @"INSERT INTO Match(Start_DateTime, End_DateTime, Match_Name)
                                   VALUES(@StartDateTime, @EndDateTime, @MatchName);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@StartDateTime", item.StartDateTime);
                command.Parameters.AddWithValue("@EndDateTime", item.EndDateTime);
                command.Parameters.AddWithValue("@MatchName", item.MatchName);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Match? Get(int id)
        {
            Match? match = null;
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        match = MapReaderToMatch(reader);
                    }
                }
            }

            CloseConnection();
            return match;
        }

        public List<Match> GetAll()
        {
            var matchList = new List<Match>();
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matchList.Add(MapReaderToMatch(reader));
                    }
                }
            }

            CloseConnection();
            return matchList;
        }

        public void Delete(int id)
        {
            string deleteQuery = "DELETE FROM Match WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        private Match MapReaderToMatch(SqliteDataReader reader)
        {
            return new Match
            {
                Id = reader.GetInt32(0),
                StartDateTime = reader.GetDateTime(1),
                EndDateTime = reader.GetDateTime(2),
                MatchName = reader.GetString(3)
            };
        }
    }
}

