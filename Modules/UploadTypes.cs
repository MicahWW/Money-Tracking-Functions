using Microsoft.VisualBasic.FileIO;

namespace Money.Modules
{
    public class TransactionRecord
    {
        public DateOnly TransactionDate { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }

        public TransactionRecord(string date, string location, string category)
        {
            this.TransactionDate = DateOnly.Parse(date);
            this.Location = location;
            this.Category = category;
        }

        public TransactionRecord(string date, string location, string category, decimal amount) : this(date, location, category)
        {
            this.Amount = amount;
        }
    }

    public class CapitalOneTracationRecord : TransactionRecord
    {
        public CapitalOneTracationRecord(string date, string location, string category, string debit, string credit) :
        base(date: date, location: location, category: category)
        {
            if (string.IsNullOrEmpty(debit) && !string.IsNullOrEmpty(credit))
                this.Amount = decimal.Parse(credit);
            else if (string.IsNullOrEmpty(credit) && !string.IsNullOrEmpty(debit))
                this.Amount = decimal.Parse(debit);
            // This shouldn't happen
            else
                this.Amount = 0;
        }

        public static List<TransactionRecord> ParseCsv(Stream stream)
        {
            var result = new List<TransactionRecord>();

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
                        result.Add(new CapitalOneTracationRecord(columns[0], columns[3], columns[4], columns[5], columns[6]));
                }
            }

            return result;
        }
    }

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