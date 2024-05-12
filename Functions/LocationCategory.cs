using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Functions
{
    public class LocationCategory
    {
        private readonly ILogger<LocationCategory> _logger;

        public LocationCategory(ILogger<LocationCategory> logger)
        {
            _logger = logger;
        }

        [Function("LocationCategory")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location-category")] HttpRequest req)
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            var table_locationCategoryDefaults = Environment.GetEnvironmentVariable("table-locationCategoryDefaults");
            string? query = req.Query["locationName"];

            if(String.IsNullOrEmpty(query))
                cmd.CommandText = $"SELECT * FROM {table_locationCategoryDefaults}";
            else
                cmd.CommandText = $"SELECT * FROM {table_locationCategoryDefaults} WHERE location = \"{query}\"";

            var rdr = cmd.ExecuteReader();
            var result = new List<Dictionary<string, int>>();

            while(rdr.Read())
                result.Add(new Dictionary<string, int>() { {(string)rdr[0], (int)rdr[1]} });

            rdr.Close();
            conn.Close();

            return new OkObjectResult(result);
        }
    }
}
