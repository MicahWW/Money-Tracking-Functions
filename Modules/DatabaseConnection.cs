using MySql.Data.MySqlClient;

namespace Money.Modules
{
    /// <summary>
    /// Helper functions related to to connecting to the database.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <param name="withOutDatabase">If the connection returned should
        /// connect to the specific database. Generally only needed before the
        /// database has been created.</param>
        /// <returns>A MySqlConnection connected to database that is defined in
        /// the system's environment variables.</returns>
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