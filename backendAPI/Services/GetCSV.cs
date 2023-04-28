using Microsoft.VisualBasic.FileIO;

namespace backendAPI.Services
{
    public static class GetCSV
    {
        public static Dictionary<string, string> GetLocationHourRangeFromCSV()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (TextFieldParser parser = new TextFieldParser("data.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    string location = fields[0];
                    string hourRange = fields[1];
                    if (dict.ContainsKey(location))
                    {
                        dict[location] = hourRange;
                    }
                    else
                    {
                        dict.Add(location, hourRange);
                    }
                    Console.WriteLine($"Location: {location}, Hour Range: {hourRange}");
                }
            }
            return dict;
        }
    }
}
