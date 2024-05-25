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
            string? type = req.Query["type"];
            string? startDate = req.Query["startDate"];
            string? endDate = req.Query["endDate"];
            string whereDateRange = "";

            if (string.IsNullOrEmpty(type))
                return new ErrorResponse("No type was passed", 515);
            type = type.ToLower();
            if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                return new ErrorResponse("An end date was given with no start date.", 515);
            if (string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(startDate))
                return new ErrorResponse("A start date was given with no end date.", 515);
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateOnly beginDate = DateOnly.Parse(startDate);
                DateOnly lastDate = DateOnly.Parse(endDate);


                whereDateRange = $"WHERE transaction_date BETWEEN '{beginDate:yyyy-MM-dd}' AND '{lastDate:yyyy-MM-dd}' ";
            }

            var categories = CategoriesTable.GetCategories();

            using(var conn = DatabaseConnection.CreateConnection())
            {
                using(var cmd = new MySqlCommand("", conn))
                {
                    switch (type)
                    {
                        case "sunburst":
                        case "pie":
                            cmd.CommandText = 
                                "SELECT " +
                                "  location, category_id, SUM(amount), COUNT(*) " +
                                "FROM " +
                                $"  {SystemVariables.TableExpenseItems} " +
                                whereDateRange +
                                "GROUP BY " +
                                "  location, category_id";

                            var rdr = cmd.ExecuteReader();

                            var result = new List<VisualizeRecord>();

                            // this adds the categories for the items to leaf off of
                            categories.ForEach(x =>
                            {
                                result.Add(new VisualizeRecord(x.label));
                            });
                            while(rdr.Read())
                            {
                                var category_find = categories.Find(x => x.id == (int)rdr[1]);
                                string category_name = category_find != null ? category_find.label : "error";
                                // (int)(long) : this is being used because MySQL is returning a int64
                                result.Add(new VisualizeRecord((string)rdr[0], category_name, (decimal)rdr[2], (int)(long)rdr[3]));
                            }
                            
                            return new OkObjectResult(result);
                        default:
                            return new ErrorResponse($"Given type of {type} is not allowed", 515);
                    }
                }
            }
        }

        public class VisualizeRecord
        {
            public string location { get; set; }
            public string? category { get; set; }
            public decimal amount { get; set; }
            public int count { get; set; }

            public VisualizeRecord(string location, string category, decimal amount, int count)
            {
                this.location = location;
                this.category = category;
                this.amount = amount;
                this.count = count;
            }

            public VisualizeRecord(string category)
            {
                this.location = category;
            }
        }
    }
}
