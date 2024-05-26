using Money.Modules;
using MySql.Data.MySqlClient;

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

            public ItemsRecord(int id, string location, decimal amount, int category_id, string transaction_date)
            {
                this.id = id;
                this.location = location;
                this.amount = amount;
                this.category_id = category_id;
                this.transaction_date = DateOnly.Parse(transaction_date);
            }

            public ItemsRecord(int id, string location, decimal amount, int category_id, DateTime transaction_date)
            {
                this.id = id;
                this.location = location;
                this.amount = amount;
                this.category_id = category_id;
                this.transaction_date = DateOnly.FromDateTime(transaction_date);
            }
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
                                (DateTime)rdr[4]
                            )
                        );

                    return result;
                }
            }
        }

        public static void InsertItems(List<TransactionRecord> items)
        {
            var categories = CategoriesTable.GetCategories();
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
                        var category_find = categories.Find(x => x.label == items[i].Category);
                        // if the category couldn't be found in the list give it an id of "No Category"
                        int category_id = category_find != null ? category_find.id : 1;

                        var name_find = locationNames.Find(x => x.provider_name == items[i].Location);
                        // if the name is not in the table then use the provided name
                        string shortName = name_find != null ? name_find.name : items[i].Location;

                        cmd.Parameters["@location"].Value = shortName;
                        cmd.Parameters["@amount"].Value = items[i].Amount;
                        cmd.Parameters["@categoryId"].Value = category_id;
                        cmd.Parameters["@date"].Value = items[i].TransactionDate.ToString("yyyy-MM-dd");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
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