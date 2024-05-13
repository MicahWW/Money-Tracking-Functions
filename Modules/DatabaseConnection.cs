using MySql.Data.MySqlClient;

namespace Money.Modules
{
    public class DatabaseConnection
    {
        public static MySqlConnection CreateConnection(bool withOutDatabase=false)
        {
            MySqlConnection conn;
            string myConnectionString = $"server={SystemVariables.MySqlHostName};" +
                $"uid={SystemVariables.MySqlUserName};" + 
                $"pwd={SystemVariables.MySqlPassword};";

            if(!withOutDatabase)
                myConnectionString += $"database={SystemVariables.MySqlDatabase}";

            conn = new MySqlConnection(myConnectionString);
            // TODO: error handeling for SQL login errors
            conn.Open();

            return conn;
        }
    }
}