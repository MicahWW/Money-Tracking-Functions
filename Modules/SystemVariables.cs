namespace Money.Modules
{
    /// <summary>
    /// This is used to get environment variables and make sure that the code
    /// for checking for errors for the requested environment variables is
    /// consistent. The environment variables are retrieved each time so they
    /// can be dynamic.
    /// </summary>
    public class SystemVariables
    {
        /// <summary>
        /// This helps keep the error detection and handling for all of the
        /// environment variables in one place.
        /// </summary>
        /// <param name="name">The name of the environment variable</param>
        /// <returns>The environment variable from the system.</returns>
        /// <exception cref="SystemVariablesException"></exception>
        private static string GetSystemVariable(string name)
        {
            string? result = Environment.GetEnvironmentVariable(name);

            // Check if the environment variable were retrieved
            if (string.IsNullOrEmpty(result))
                throw new SystemVariablesException($"The variable {name} was not found or empty.");

            // Check if the KeyVault reference didn't get resolved
            if (result.Contains("Microsoft.KeyVault"))
                throw new SystemVariablesException($"The variable {name} did not get resolved via the Key Vault.");

            return result;
        }

        public static string MySqlHostName { get { return GetSystemVariable("mysql-host"); } }

        public static string MySqlUserName { get { return GetSystemVariable("mysql-user"); } }

        public static string MySqlPassword { get { return GetSystemVariable("mysql-pass"); } }

        public static string MySqlDatabase { get { return GetSystemVariable("mysql-db"); } }

        public static string TableCategories { get { return GetSystemVariable("table-categories"); } }

        public static string TableLocationCategories { get { return GetSystemVariable("table-locationCategoryDefaults"); } }

        public static string TableExpenseItems { get { return GetSystemVariable("table-expenseItems"); } }

        public static string TableLocationNames { get { return GetSystemVariable("table-locationLongToShortName"); } }

        /// <summary>
        /// The Environment variables either weren't there or couldn't be
        /// interpreted.
        /// </summary>
        public class SystemVariablesException : Exception
        {
            public SystemVariablesException(string message) : base(message) { }
        }
    }
}