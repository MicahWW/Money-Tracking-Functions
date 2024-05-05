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

            if(String.IsNullOrEmpty(req.Query["locationName"]))
                cmd.CommandText = $"SELECT * FROM {System.Environment.GetEnvironmentVariable("table-locationCategoryDefaults")}";
            else
                cmd.CommandText = $"SELECT * FROM {System.Environment.GetEnvironmentVariable("table-locationCategoryDefaults")}" +
                    $" WHERE location = \"{req.Query["locationName"]}\"";

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
