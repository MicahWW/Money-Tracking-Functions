using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Modules;
using Money.Tables;
using MySql.Data.MySqlClient;

namespace Money.Function
{
    public class VisualizeSpending
    {
        private readonly ILogger<VisualizeSpending> _logger;

        public VisualizeSpending(ILogger<VisualizeSpending> logger)
        {
            _logger = logger;
        }

        [Function("Visualize")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "visualize-spending")] HttpRequest req)
        {
            #region Check query parameters
            string? type = req.Query["type"];
            string? startDate = req.Query["startDate"];
            string? endDate = req.Query["endDate"];
            string whereDateRange = "";

            if (string.IsNullOrEmpty(type))
                return new ErrorResponse("No type was passed", 515);
            if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                return new ErrorResponse("An end date was given with no start date.", 515);
            if (string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(startDate))
                return new ErrorResponse("A start date was given with no end date.", 515);

            // adds the option to search within a date range
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateOnly beginDate = DateOnly.Parse(startDate);
                DateOnly lastDate = DateOnly.Parse(endDate);

                whereDateRange = $"AND transaction_date BETWEEN '{beginDate:yyyy-MM-dd}' AND '{lastDate:yyyy-MM-dd}' ";
            }
            #endregion Check query parameters

            var categories = CategoriesTable.GetCategories();

            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    // pie charts and sunburst charts use the same data, execpt
                    // pie doesn't need the categories.
                    if (string.Compare(type, "sunburst", true) == 0 ||
                        string.Compare(type, "pie", true) == 0)
                    {
                        // Gets all of the items in the ExpenseItems table that
                        // are a negative value (items where money was spent, 
                        // not recieved) grouped by the location and category.
                        // The amount is made postive so it can be dispalyed.
                        cmd.CommandText =
                            "SELECT " +
                            "  location, category_id, SUM(amount) * -1, COUNT(*) " +
                            "FROM " +
                            $"  {SystemVariables.TableExpenseItems} " +
                            "WHERE " +
                            "  amount < 0 " +
                            // adds the optional date restriction
                            whereDateRange +
                            "GROUP BY " +
                            "  location, category_id";

                        var rdr = cmd.ExecuteReader();

                        var result = new List<VisualizeRecord>();

                        // this adds the categories for the items to leaf off of
                        // in the sunburst chart
                        categories.ForEach(x =>
                        {
                            result.Add(new VisualizeRecord(x.label));
                        });
                        while (rdr.Read())
                        {
                            var category_find = categories.Find(x => x.id == (int)rdr[1]);
                            string category_name = category_find != null ? category_find.label : "error";
                            // (int)(long) : this is being used because MySQL is returning a int64
                            result.Add(new VisualizeRecord((string)rdr[0], category_name, (decimal)rdr[2], (int)(long)rdr[3]));
                        }

                        return new OkObjectResult(result);
                    }
                    else
                    {
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
