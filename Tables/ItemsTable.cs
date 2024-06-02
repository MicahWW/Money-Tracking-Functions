using Money.Modules;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic.FileIO;

namespace Money.Tables
{
    public class ItemsTable
    {
        public class ItemsRecord
        {
            public int id { get; set; }
            public string location { get; set; }
            public decimal amount { get; set; }
            public int category_id { get; set; }
            public DateOnly transaction_date { get; set; }

            public ItemsRecord(int id, string location, decimal amount, int category_id, DateOnly transaction_date)
            {
                this.id = id;
                this.location = location;
                this.amount = amount;
                this.category_id = category_id;
                this.transaction_date = transaction_date;
            }

            public ItemsRecord(int id, string location, decimal amount, string category, DateOnly transaction_date)
            {
                this.id = id;
                this.location = location;
                this.amount = amount;
                this.transaction_date = transaction_date;

                var categories = CategoriesTable.GetCategories();
                var category_find = categories.Find(x => x.label == category);
                // if the category couldn't be found in the list give it an id of "No Category"
                this.category_id = category_find != null ? category_find.id : 1;

            }

            public ItemsRecord(string location, decimal amount, string category, DateOnly transaction_date) :
            this(-1, location, amount, category, transaction_date)
            { }
        }

        public static List<ItemsRecord> GetItems()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    cmd.CommandText = $"SELECT * FROM {SystemVariables.TableExpenseItems}";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<ItemsRecord>();
                    while (rdr.Read())
                        result.Add(
                            new ItemsRecord(
                                (int)rdr[0], (string)rdr[1],
                                (decimal)rdr[2],
                                (int)rdr[3],
                                DateOnly.FromDateTime((DateTime)rdr[4])
                            )
                        );

                    return result;
                }
            }
        }

        public static void InsertItems(List<ItemsRecord> items)
        {
            var locationNames = LocationNamesTable.GetLocationNames();

            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_expenseItems = SystemVariables.TableExpenseItems;

                    // for now only keeping items in the file in the database
                    cmd.CommandText = $"DELETE FROM {table_expenseItems}";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText =
                        $"INSERT INTO {table_expenseItems} " +
                        "  (location, amount, category_id, transaction_date) " +
                        "VALUES " +
                        "  (@location, @amount, @categoryId, @date)";
                    cmd.Parameters.AddWithValue("@location", "One");
                    cmd.Parameters.AddWithValue("@amount", 1.0);
                    cmd.Parameters.AddWithValue("@categoryId", 1);
                    cmd.Parameters.AddWithValue("@date", DateOnly.Parse("01/01/1970"));
                    cmd.Prepare();

                    for (int i = 0; i < items.Count; i++)
                    {
                        var name_find = locationNames.Find(x => x.provider_name == items[i].location);
                        // if the name is not in the table then use the provided name
                        string shortName = name_find != null ? name_find.name : items[i].location;

                        cmd.Parameters["@location"].Value = shortName;
                        cmd.Parameters["@amount"].Value = items[i].amount;
                        cmd.Parameters["@categoryId"].Value = items[i].category_id;
                        cmd.Parameters["@date"].Value = items[i].transaction_date.ToString("yyyy-MM-dd");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static async Task UploadData(Stream stream, string contentType, int contentLength)
        {
            StringReader sr = await HttpRequestTools.ReadBodyAsync(stream, contentLength);

            var result = new List<ItemsRecord>();
            switch(contentType)
            {
                case "text/csv":
                    using (TextFieldParser parser = new TextFieldParser(sr))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        // gets rid of headers
                        parser.ReadFields();

                        while (!parser.EndOfData)
                        {
                            string[]? columns = parser.ReadFields();
                            if (columns != null) 
                            {
                                decimal amount = 0;
                                if (string.IsNullOrEmpty(columns[5]) && !string.IsNullOrEmpty(columns[6]))
                                    amount = decimal.Parse(columns[6]);
                                else if (string.IsNullOrEmpty(columns[6]) && !string.IsNullOrEmpty(columns[5]))
                                    amount = decimal.Parse(columns[5]);

                                result.Add(new ItemsRecord(columns[3], amount, columns[4], DateOnly.Parse(columns[0])));
                            }
                        }
                    }
                    break;
            }

            InsertItems(result);
        }

        public static void Setup()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_expenseItems = SystemVariables.TableExpenseItems;
                    var table_categories = SystemVariables.TableCategories;

                    cmd.CommandText =
                        $"CREATE TABLE IF NOT EXISTS {table_expenseItems} (" +
                        "  id int NOT NULL AUTO_INCREMENT," +
                        "  location varchar(255) NOT NULL," +
                        "  amount decimal(15, 2) NOT NULL," +
                        "  category_id int NOT NULL," +
                        "  transaction_date date NOT NULL," +
                        "  PRIMARY KEY (id)," +
                        $"  FOREIGN KEY (category_id) REFERENCES {table_categories}(id)" +
                        ")";

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}