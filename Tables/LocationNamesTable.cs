using Money.Modules;
using MySql.Data.MySqlClient;

namespace Money.Tables
{
    public class LocationNamesTable
    {
        public class LocationnamesRecord
        {
            public string provider_name { get; set; }
            public string name { get; set; }

            public LocationnamesRecord(string provider_name, string name)
            {
                this.provider_name = provider_name;
                this.name = name;
            }
        }

        public static List<LocationnamesRecord> GetLocationNames(string? query="")
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    var table_locationLongToShortName = Environment.GetEnvironmentVariable("table-locationLongToShortName");
                    cmd.CommandText = $"SELECT * FROM {table_locationLongToShortName}";
                    if (!string.IsNullOrEmpty(query))
                        cmd.CommandText += $" WHERE provider_name = \"{query}\"";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<LocationnamesRecord>();
                    while(rdr.Read())
                        result.Add(new LocationnamesRecord((string)rdr[0], (string)rdr[1]));

                    return result;
                }
            }
        }

        public static void Setup()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    var table_locationLongToShortName = Environment.GetEnvironmentVariable("table-locationLongToShortName");

                    cmd.CommandText =
                        $"CREATE TABLE IF NOT EXISTS {table_locationLongToShortName} (" +
                        "  provider_name varchar(255) NOT NULL," +
                        "  name varchar(255) DEFAULT NULL," +
                        "  PRIMARY KEY (provider_name)" +
                        ")";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}