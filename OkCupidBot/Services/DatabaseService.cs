using OkCupidBot.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Services
{
    public class DatabaseService : BaseService
    {
        public const string DatabaseFileName = "OkCupid.db3";
        public const string HistoryTable = @"CREATE TABLE IF NOT EXISTS [History] (
	[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	[Username] NVARCHAR(128)  NOT NULL,
	[Url] NVARCHAR(128)  NOT NULL,
	[Message] TEXT  NULL,
	[Date] DATETIME  NOT NULL)";

        public DatabaseService()
        {
            CreateDatabase();
        }

        public bool UserExists(string username)
        {
            using (SQLiteConnection con = new SQLiteConnection(string.Format("data source={0}", DatabaseFileName)))
            using (SQLiteCommand com = new SQLiteCommand(con))
            {
                con.Open();
                com.CommandText = "SELECT COUNT(Id) FROM History WHERE Username = @Username";
                com.Parameters.AddWithValue("@Username", username);
                int results = Convert.ToInt32(com.ExecuteScalar());
                return results > 1;
            }
        }

        public void AddUser(Profile prof, string message)
        {
            using (SQLiteConnection con = new SQLiteConnection(string.Format("data source={0}", DatabaseFileName)))
            using (SQLiteCommand com = new SQLiteCommand(con))
            {
                con.Open();
                com.CommandText = "INSERT INTO History (Username, Url, Message, Date) VALUES(@Username, @Url, @Message, @Date)";
                com.Parameters.AddWithValue("@Username", prof.Username);
                com.Parameters.AddWithValue("@Url", prof.ProfilePage.ToString());
                com.Parameters.AddWithValue("@Message", message);
                com.Parameters.AddWithValue("@Date", DateTime.Now);
                int result = com.ExecuteNonQuery();
            }
        }

        private void CreateDatabase()
        {
            if (!File.Exists(DatabaseFileName))
            {
                SQLiteConnection.CreateFile(DatabaseFileName);
            }

            using (SQLiteConnection con = new SQLiteConnection(string.Format("data source={0}", DatabaseFileName)))
            using (SQLiteCommand com = new SQLiteCommand(con))
            {
                con.Open();
                com.CommandText = HistoryTable;
                com.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
