using System.Text.Json.Serialization;

namespace StreamMasterDomain.Pagination
{
    public class ProgrammeParameters : QueryStringParameters
    {
        public ProgrammeParameters()
        {
            OrderBy = "name";
        }

        [JsonIgnore]
        public int Count => Last - First + 1;
        public int First { get; set; } = 0;
        public int Last { get; set; } = 1;
        public string Name { get; set; } = string.Empty;
    }
}