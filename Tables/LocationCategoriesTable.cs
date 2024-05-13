using Money.Modules;
using MySql.Data.MySqlClient;

namespace Money.Tables
{
    public class LocationCategoryTable
    {
        public class LocationCategoryRecord
        {
            public string location { get; set; }
            public int category_id { get; set; }

            public LocationCategoryRecord(string location, int category_id)
            {
                this.location = location;
                this.category_id = category_id;
            }
        }

        public static List<LocationCategoryRecord> GetLocationCategories(string? query="")
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    var table_locationCategoryDefaults = SystemVariables.TableLocationCategories;
                    cmd.CommandText = $"SELECT * FROM {table_locationCategoryDefaults}";
                    if (!string.IsNullOrEmpty(query))
                        cmd.CommandText += $" WHERE location = \"{query}\"";

                    var rdr = cmd.ExecuteReader();


                    var result = new List<LocationCategoryRecord>();
                    while(rdr.Read())
                        result.Add(new LocationCategoryRecord((string)rdr[0], (int)rdr[1]));

                    return result;
                }
            }
        }

        public static void Setup()
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;

            var table_locationCategoryDefaults = SystemVariables.TableLocationCategories;
            var table_categories = SystemVariables.TableCategories;
            
            cmd.CommandText = 
                $"CREATE TABLE IF NOT EXISTS {table_locationCategoryDefaults} (" +
                "  location VARCHAR(255) NOT NULL," +
                "  category_id int NOT NULL," +
                "  PRIMARY KEY (location)," +
                $" FOREIGN KEY (category_id) REFERENCES {table_categories}(id)" +
                ")";
            cmd.ExecuteNonQuery();
        }
    }
}