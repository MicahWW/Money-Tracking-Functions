using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Functions
{
    public class Categories
    {
        private readonly ILogger<Categories> _logger;

        public Categories(ILogger<Categories> logger)
        {
            _logger = logger;
        }

        [Function("Categories")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")] HttpRequest req)
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand($"SELECT * FROM {Environment.GetEnvironmentVariable("table-categories")}", conn);
            var rdr = cmd.ExecuteReader();

            var result = new List<Category>();
            while(rdr.Read())
                result.Add(new Category(rdr[0], rdr[1]));

            rdr.Close();
            conn.Close();

            return new OkObjectResult(result);
        }

        public class Category
        {
            public int id { get; set; }
            public string? label { get; set; }

            public Category(Object id, Object label)
            {
                this.id = (int)id;
                this.label = (string)label;
            }
        }
    }
}
