using System.Data;
using ProjectBotenReservering.Core.Interfaces.Database;

namespace ProjectBotenReservering.Core.Data.Database.Schema
{
    public class SqliteSchemaInitializer : ISchemaInitializer
    {
        public void Initialize(IDbConnection connection)
        {
            List<string> tableScripts = new List<string>
            {
                @"CREATE TABLE IF NOT EXISTS Boat (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Name] STRING NOT NULL,
                    [Steering_Wheel] BOOLEAN NOT NULL,
                    [Seats] INT NOT NULL,
                    [Level] INT NOT NULL,
                    [Type] CHAR NOT NULL,
                    [Kg] INT NOT NULL,
                    [Operational] BOOLEAN NOT NULL,
                    [Club] VARCHAR)",

                @"CREATE TABLE IF NOT EXISTS Role (
                    [Name] VARCHAR(50) NOT NULL PRIMARY KEY UNIQUE)",

                @"CREATE TABLE IF NOT EXISTS ManagementTask (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Name] VARCHAR(50) NOT NULL)",

                @"CREATE TABLE IF NOT EXISTS WindConstraint (
                    [Windforce] INT NOT NULL PRIMARY KEY,
                    [Min_Scull_level] INT NOT NULL,
                    [Min_Sweep_level] INT NOT NULL)",

                @"CREATE TABLE IF NOT EXISTS Client (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Full_Name] VARCHAR NOT NULL,
                    [Email] VARCHAR NOT NULL UNIQUE,
                    [Scull_level] INT,
                    [Sweep_level] INT,
                    [Club] VARCHAR,
                    [Approved] BOOLEAN NOT NULL DEFAULT 0,
                    [Password_Hash] VARCHAR NOT NULL)",

                @"CREATE TABLE IF NOT EXISTS Reservation (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Created_At] DATETIME NOT NULL,
                    [Start_Time] DATETIME NOT NULL,
                    [End_Time] DATETIME NOT NULL,
                    [Client_Id] INT NOT NULL,
                    [Boat_Id] INT NOT NULL,
                    [Approved] BOOLEAN NOT NULL,
                    [Active] BOOLEAN NOT NULL DEFAULT 1,
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))",

                @"CREATE TABLE IF NOT EXISTS DamageReport (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Client_Id] INT NOT NULL,
                    [Boat_Id] INT NOT NULL,
                    [Damage_Information] LONGVARCHAR NOT NULL,
                    [Date] DATETIME NOT NULL,
                    [Approved] BOOLEAN NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))",

                @"CREATE TABLE IF NOT EXISTS DamageReportPhotos (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [DamageReport_Id] INT NOT NULL,
                    [Url] VARCHAR NOT NULL,
                    FOREIGN KEY (DamageReport_Id) REFERENCES DamageReport(Id))",

                @"CREATE TABLE IF NOT EXISTS Match (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Start_DateTime] DATETIME NOT NULL,
                    [End_DateTime] DATETIME NOT NULL,
                    [Match_Name] VARCHAR NOT NULL)",

                @"CREATE TABLE IF NOT EXISTS Client_ManagementTask (
                    [Client_Id] INT NOT NULL,
                    [Management_Task_Id] INT NOT NULL,
                    PRIMARY KEY (Client_Id, Management_Task_Id),
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Management_Task_Id) REFERENCES ManagementTask(Id))",

                @"CREATE TABLE IF NOT EXISTS Client_Reservation (
                    [Client_Id] INT NOT NULL,
                    [Reservation_Id] INT NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Reservation_Id) REFERENCES Reservation(Id))",

                @"CREATE TABLE IF NOT EXISTS Client_Role (
                    [Role_Name] VARCHAR(50) NOT NULL,
                    [Client_Id] INT NOT NULL,
                    PRIMARY KEY (Role_Name, Client_Id),
                    FOREIGN KEY (Role_Name) REFERENCES Role(Name),
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id))",

                @"CREATE TABLE IF NOT EXISTS Role_ManagementTask (
                    [Role_Id] VARCHAR(50) NOT NULL,
                    [ManagementTask_Id] INT NOT NULL,
                    PRIMARY KEY (Role_Id, ManagementTask_Id),
                    FOREIGN KEY (Role_Id) REFERENCES Role(Name),
                    FOREIGN KEY (ManagementTask_Id) REFERENCES ManagementTask(Id))",

                @"CREATE TABLE IF NOT EXISTS Reservation_Match (
                    [Match_Id] INT NOT NULL,
                    [Reservation_Id] INT NOT NULL,
                    [Team_Name] VARCHAR NOT NULL,
                    PRIMARY KEY(Match_Id, Reservation_Id),
                    FOREIGN KEY(Match_Id) REFERENCES Match(Id),
                    FOREIGN KEY(Reservation_Id) REFERENCES Reservation(Id))"
            };

            foreach (string script in tableScripts)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}