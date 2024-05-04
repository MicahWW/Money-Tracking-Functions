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

        [Function("DatabaseSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/database")] HttpRequest req)
        {
            var conn = DatabaseConnection.CreateConnection(true);
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"CREATE DATABASE {System.Environment.GetEnvironmentVariable("mysql-db")}";

            cmd.ExecuteNonQuery();

            return new OkObjectResult("database setup");
        }
    }
}
