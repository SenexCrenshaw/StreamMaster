using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Pagination
{
    public class IconFileParameters : QueryStringParameters
    {
        public IconFileParameters()
        {
            OrderBy = "name desc";
        }

        [JsonIgnore]
        public int Count => Last - First + 1;
        public int First { get; set; } = 0;
        public int Last { get; set; } = 1;
    }
}