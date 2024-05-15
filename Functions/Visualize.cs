using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Modules;
using Money.Tables;
using MySql.Data.MySqlClient;

namespace Money.Function
{
    public class Visualize
    {
        private readonly ILogger<Visualize> _logger;

        public Visualize(ILogger<Visualize> logger)
        {
            _logger = logger;
        }

        [Function("Visualize")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "visualize")] HttpRequest req)
        {
            // right now data is meant for a sunburst chart



            var categories = CategoriesTable.GetCategories();

            using(var conn = DatabaseConnection.CreateConnection())
            {
                using(var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT location, category_id, SUM(amount) FROM items GROUP BY location, category_id";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<VisualizeRecord>();
                    categories.ForEach(x =>
                    {
                        result.Add(new VisualizeRecord(x.label));
                    });
                    while(rdr.Read())
                    {
                        var category_find = categories.Find(x => x.id == (int)rdr[1]);
                        string category_name = category_find != null ? category_find.label : "error";
                        result.Add(new VisualizeRecord((string)rdr[0], category_name, (decimal)rdr[2]));
                    }
                    
                    return new OkObjectResult(result);
                }
            }
        }

        public class VisualizeRecord
        {
            public string location {get; set;}
            public string category {get; set;}
            public decimal amount {get; set;}

            public VisualizeRecord(string location, string category, decimal amount)
            {
                this.location = location;
                this.category = category;
                this.amount = amount;
            }

            public VisualizeRecord(string category)
            {
                this.location = category;
                this.category = "";
                this. amount = 0;
            }
        }
    }
}
