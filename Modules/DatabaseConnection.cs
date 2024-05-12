using MySql.Data.MySqlClient;

namespace Money.Modules
{
    public class DatabaseConnection
    {
        public static MySqlConnection CreateConnection(bool withOutDatabase=false)
        {
            MySqlConnection conn;
            string myConnectionString;

            if(!withOutDatabase)
                myConnectionString = $"server={Environment.GetEnvironmentVariable("mysql-host")};" + 
                    $"uid={Environment.GetEnvironmentVariable("mysql-user")};" +
                    $"pwd={Environment.GetEnvironmentVariable("mysql-pass")};" +
                    $"database={Environment.GetEnvironmentVariable("mysql-db")}";
            else
                myConnectionString = $"server={Environment.GetEnvironmentVariable("mysql-host")};" + 
                    $"uid={Environment.GetEnvironmentVariable("mysql-user")};" +
                    $"pwd={Environment.GetEnvironmentVariable("mysql-pass")};";

            conn = new MySqlConnection(myConnectionString);
            // TODO: error handeling for SQL login errors
            conn.Open();

            return conn;
        }
    }
}