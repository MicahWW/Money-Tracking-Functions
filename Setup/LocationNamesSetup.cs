using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class LocationNamesSetup
    {
        private readonly ILogger<LocationNamesSetup> _logger;

        public LocationNamesSetup(ILogger<LocationNamesSetup> logger)
        {
            _logger = logger;
        }

        public static void Setup()
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {System.Environment.GetEnvironmentVariable("table-locationLongToShortName")}" +
            " (provider_name varchar(255) NOT NULL, name varchar(255) DEFAULT NULL, PRIMARY KEY (provider_name))";
            cmd.ExecuteNonQuery();
        }

        [Function("LocationNamesSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/location-names")] HttpRequest req)
        {
            Setup();

            return new OkObjectResult("Location name table created");
        }
    }
}
