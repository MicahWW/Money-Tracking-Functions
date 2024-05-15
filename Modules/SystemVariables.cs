namespace Money.Modules
{
    public class SystemVariables
    {
        private static string GetSystemVariable(string name)
        {
            string? result = Environment.GetEnvironmentVariable(name);

            // Check if the environment variable were retrieved
            if (string.IsNullOrEmpty(result))
                throw new MissingSystemVariableException(name);

            // Check if the KeyVault reference didn't get resolved
            if (result.Contains("Microsoft.KeyVault"))
                throw new KeyVaultReferenceException(name);

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

        public class SystemVariablesException : Exception
        {
            public SystemVariablesException(string message) : base(message) { }
        }


        private class MissingSystemVariableException : SystemVariablesException
        {
            public MissingSystemVariableException(string variable) : base
            (
                $"The variable {variable} was not found or empty."
            )
            { }
        }

        private class KeyVaultReferenceException : SystemVariablesException
        {
            public KeyVaultReferenceException(string variable) : base
            (
                $"The variable {variable} did not get resolved via the Key Vault."
            )
            { }
        }
    }
}