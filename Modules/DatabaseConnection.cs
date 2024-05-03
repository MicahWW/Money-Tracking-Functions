using MySql.Data.MySqlClient;

namespace Money.Modules
{
    public class DatabaseConnection
    {
        public static MySqlConnection CreateConnection()
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = $"server={System.Environment.GetEnvironmentVariable("mysql-host")};" + 
                $"uid={System.Environment.GetEnvironmentVariable("mysql-user")};" +
                $"pwd={System.Environment.GetEnvironmentVariable("mysql-pass")};" +
                $"database={System.Environment.GetEnvironmentVariable("mysql-db")}";

            conn = new MySqlConnection(myConnectionString);
            // TODO: error handeling for SQL login errors
            conn.Open();

            return conn;
        }
    }
}