using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class DatabaseSetup
    {
        private readonly ILogger<DatabaseSetup> _logger;

        public DatabaseSetup(ILogger<DatabaseSetup> logger)
        {
            _logger = logger;
        }


        public static void Setup()
        {
            var conn = DatabaseConnection.CreateConnection(true);
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS {Environment.GetEnvironmentVariable("mysql-db")}";

            cmd.ExecuteNonQuery();
        }

        [Function("DatabaseSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/database")] HttpRequest req)
        {
            Setup();

            return new OkObjectResult("database setup");
        }
    }
}
