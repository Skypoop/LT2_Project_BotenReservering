using ProjectBotenReservering.Core.Data.Helpers;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data
{
    public abstract class DatabaseConnection : IAsyncDisposable, IDisposable
    {
        protected SqliteConnection Connection { get; }

        public DatabaseConnection()
        {
            string? databaseName = ConnectionHelper.ConnectionStringValue("RoeiverenigingDB");
#if MACCATALYST
            string writableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(writableDirectory); 
#else
            string writableDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
            string dbpath = "Data Source=" + Path.Combine(writableDirectory, databaseName);
            Connection = new SqliteConnection(dbpath);
        }

        protected async Task OpenConnectionAsync()
        {
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
        }

        protected async Task CloseConnectionAsync()
        {
            if (Connection.State != System.Data.ConnectionState.Closed)
            {
                await Connection.CloseAsync();
            }
        }

        public async Task CreateTableAsync(string commandText)
        {
            await OpenConnectionAsync();
            using (SqliteCommand command = Connection.CreateCommand())
            {
                command.CommandText = commandText;
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task InsertMultipleWithTransactionAsync(List<string> linesToInsert)
        {
            await OpenConnectionAsync();
            SqliteTransaction transaction = (SqliteTransaction)await Connection.BeginTransactionAsync();
            try
            {
                foreach (string line in linesToInsert)
                {
                    using (SqliteCommand command = Connection.CreateCommand())
                    {
                        command.CommandText = line;
                        command.Transaction = (SqliteTransaction?)transaction;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseConnectionAsync();
            await Connection.DisposeAsync();
        }

        public void Dispose()
        {
            CloseConnectionAsync().GetAwaiter().GetResult();
            Connection.Dispose();
        }
    }
}