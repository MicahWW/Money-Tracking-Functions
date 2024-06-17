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
        /// <param name="withOutDatabase">
        /// If the connection returned should connect to the specific database.
        /// Generally only needed before the database has been created.
        /// </param>
        /// <returns>
        /// A MySqlConnection connected to database that is defined in the
        /// system's environment variables.
        /// </returns>
        public static MySqlConnection CreateConnection(bool withOutDatabase = false)
        {
            MySqlConnection conn;
            string myConnectionString = $"server={SystemVariables.MySqlHostName};" +
                $"uid={SystemVariables.MySqlUserName};" +
                $"pwd={SystemVariables.MySqlPassword};";

            if (!withOutDatabase)
                myConnectionString += $"database={SystemVariables.MySqlDatabase}";

            conn = new MySqlConnection(myConnectionString);
            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                throw new DatabaseConnectionExecption(ex);
            }

            return conn;
        }

        public class DatabaseConnectionExecption : Exception
        {
            public int MySQLErrorNumber { get; private set; }
            public string MySQLErrorMessage { get; private set; }

            public DatabaseConnectionExecption(MySqlException ex) : base
            (
                $"MySQL error code: {ex.Number} - {ex.Message}"
            )
            {
                this.MySQLErrorNumber = ex.Number;
                this.MySQLErrorMessage = ex.Message;
            }
        }
    }
}