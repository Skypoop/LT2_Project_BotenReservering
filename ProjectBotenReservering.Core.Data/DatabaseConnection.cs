using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Data.Helpers;

namespace ProjectBotenReservering.Core.Data
{
    public abstract class DatabaseConnection : IDisposable
    {
        protected SqliteConnection Connection { get; }

        public DatabaseConnection()
        {
            string writableDirectory;
#if MACCATALYST
            writableDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Directory.CreateDirectory(writableDirectory);
#elif ANDROID
            writableDirectory = Android.App.Application.Context.FilesDir.Path;
#else
            writableDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
#if ANDROID
            // this is a temporary fix 
            // we need to add the appsettings to the build zip
            // for it to work with android, but this is complex and low prio for now
			// since we dont really have anythng sensitive

            string? databaseName = "Roeivereniging.db3";
            string fullPath = Path.Combine(writableDirectory, databaseName);
            Connection = new SqliteConnection($"Data Source={fullPath}");
#else
            string? databaseName = ConnectionHelper.ConnectionStringValue("RoeiverenigingDB");
            string dbpath = "Data Source=" + Path.Combine(writableDirectory + databaseName);
            Connection = new SqliteConnection(dbpath);

#endif
        }



        protected void OpenConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Open) Connection.Open();
        }

        protected void CloseConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Closed) Connection.Close();
        }

        public void CreateTable(string commandText)
        {
            OpenConnection();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
        }

        public void InsertMultipleWithTransaction(List<string> linesToInsert)
        {
            OpenConnection();
            var transaction = Connection.BeginTransaction();

            try
            {
                linesToInsert.ForEach(l => Connection.ExecuteNonQuery(l));
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}
