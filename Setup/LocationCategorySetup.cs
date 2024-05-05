using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class LocationCategorySetup
    {
        private readonly ILogger<LocationCategorySetup> _logger;

        public LocationCategorySetup(ILogger<LocationCategorySetup> logger)
        {
            _logger = logger;
        }

        public static void Setup()
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {System.Environment.GetEnvironmentVariable("table-locationCategoryDefaults")}" +
                " (location VARCHAR(255) NOT NULL, category_id int NOT NULL, PRIMARY KEY (location)," +
                $" FOREIGN KEY (category_id) REFERENCES {System.Environment.GetEnvironmentVariable("table-categories")}(id))";
            cmd.ExecuteNonQuery();
        }

        [Function("LocationCategorySetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/location-category")] HttpRequest req)
        {
            Setup();

            return new OkObjectResult("Location categories setup");
        }
    }
}
