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

            var table_locationCategoryDefaults = Environment.GetEnvironmentVariable("table-locationCategoryDefaults");
            var table_categories = Environment.GetEnvironmentVariable("table-categories");
            
            cmd.CommandText = 
                $"CREATE TABLE IF NOT EXISTS {table_locationCategoryDefaults} (" +
                "  location VARCHAR(255) NOT NULL," +
                "  category_id int NOT NULL," +
                "  PRIMARY KEY (location)," +
                $" FOREIGN KEY (category_id) REFERENCES {table_categories}(id)" +
                ")";
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
