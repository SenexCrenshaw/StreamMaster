using MessagePack;

using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Pagination
{
    public class StationChannelNameParameters : QueryStringParameters
    {
        public StationChannelNameParameters()
        {
            OrderBy = "name desc";
        }

        [JsonIgnore]
        [IgnoreMember]
        public int Count => Last - First + 1;
        public int First { get; set; } = 0;
        public int Last { get; set; } = 1;
    }
}