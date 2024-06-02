using Microsoft.VisualBasic.FileIO;

namespace Money.Modules
{
    public class LocationNameRecord
    {
        public string ProviderName { get; set; }
        public string ShortName { get; set; }

        public LocationNameRecord(string providerName, string shortName)
        {
            this.ProviderName = providerName;
            this.ShortName = shortName;
        }

        public static List<LocationNameRecord> ParseCsv(Stream stream)
        {
            var result = new List<LocationNameRecord>();

            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                // gets rid of headers
                parser.ReadFields();

                while (!parser.EndOfData)
                {
                    string[]? columns = parser.ReadFields();
                    if (columns != null)
                        result.Add(new LocationNameRecord(columns[0], columns[1]));
                }
            }

            return result;
        }
    }
}