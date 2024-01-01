using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class StationChannelMap
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonPropertyName("map")]
        //[JsonConverter(typeof(SingleOrListConverter<LineupChannel>))]
        public List<LineupChannel> Map { get; set; } = [];

        [JsonPropertyName("stations")]
        //[JsonConverter(typeof(SingleOrListConverter<LineupStation>))]
        public List<LineupStation> Stations { get; set; } = [];

        [JsonPropertyName("metadata")]
        public LineupMetadata? Metadata { get; set; }
    }

    public class LineupChannel
    {
        public string ChannelNumber => $"{myChannelNumber}{(myChannelSubnumber > 0 ? $".{myChannelSubnumber}" : "")}";
        public int myChannelNumber
        {
            get
            {
                if (string.IsNullOrEmpty(Channel)) return ChannelMajor ?? AtscMajor ?? UhfVhf ?? -1;
                if (Regex.Match(Channel, @"[A-Za-z]{1}[\d]{4}").Length > 0) return int.Parse(Channel[2..]);
                if (Regex.Match(Channel, @"[A-Za-z0-9.]\.[A-Za-z]{2}").Length > 0) return -1;
                if (int.TryParse(Regex.Replace(Channel, "[^0-9.]", ""), out int number)) return number;
                else
                {
                    // if channel number is not a whole number, must be a decimal number
                    string[] numbers = Regex.Replace(Channel, "[^0-9.]", "").Replace('_', '.').Replace('-', '.').Split('.');
                    if (numbers.Length == 2)
                    {
                        return int.Parse(numbers[0]);
                    }
                }
                return -1;
            }
        }

        public int myChannelSubnumber
        {
            get
            {
                if (string.IsNullOrEmpty(Channel)) return ChannelMinor ?? AtscMinor ?? 0;
                if (!int.TryParse(Regex.Replace(Channel, "[^0-9.]", ""), out _))
                {
                    // if channel number is not a whole number, must be a decimal number
                    string[] numbers = Regex.Replace(Channel, "[^0-9.]", "").Replace('_', '.').Replace('-', '.').Split('.');
                    if (numbers.Length == 2)
                    {
                        return int.Parse(numbers[1]);
                    }
                }
                return 0;
            }
        }

        [JsonIgnore]
        public string MatchName { get; set; }

        [JsonPropertyName("stationID")]
        public string StationId { get; set; }

        [JsonPropertyName("uhfVhf")]
        public int? UhfVhf { get; set; }

        [JsonPropertyName("atscMajor")]
        public int? AtscMajor { get; set; }

        [JsonPropertyName("atscMinor")]
        public int? AtscMinor { get; set; }

        [JsonPropertyName("atscType")]
        public string AtscType { get; set; }

        [JsonPropertyName("frequencyHz")]
        public long? FrequencyHz { get; set; }

        [JsonPropertyName("polarization")]
        public string Polarization { get; set; }

        [JsonPropertyName("deliverySystem")]
        public string DeliverySystem { get; set; }

        [JsonPropertyName("modulationSystem")]
        public string ModulationSystem { get; set; }

        [JsonPropertyName("symbolrate")]
        public int? Symbolrate { get; set; }

        [JsonPropertyName("fec")]
        public string Fec { get; set; }

        [JsonPropertyName("serviceID")]
        public int? ServiceId { get; set; }

        [JsonPropertyName("networkID")]
        public int? NetworkId { get; set; }

        [JsonPropertyName("transportID")]
        public int? TransportId { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("virtualChannel")]
        public string VirtualChannel { get; set; }

        [JsonPropertyName("channelMajor")]
        public int? ChannelMajor { get; set; }

        [JsonPropertyName("channelMinor")]
        public int? ChannelMinor { get; set; }

        [JsonPropertyName("providerChannel")]
        public string ProviderChannel { get; set; }

        [JsonPropertyName("providerCallsign")]
        public string ProviderCallsign { get; set; }

        [JsonPropertyName("logicalChannelNumber")]
        public string LogicalChannelNumber { get; set; }

        [JsonPropertyName("matchType")]
        public string MatchType { get; set; }
    }

    public class LineupStation
    {
        [JsonPropertyName("stationID")]
        public string StationId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("callsign")]
        public string Callsign { get; set; }

        [JsonPropertyName("affiliate")]
        public string Affiliate { get; set; }

        [JsonPropertyName("broadcastLanguage")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] BroadcastLanguage { get; set; }

        [JsonPropertyName("descriptionLanguage")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] DescriptionLanguage { get; set; }

        [JsonPropertyName("broadcaster")]
        public StationBroadcaster Broadcaster { get; set; }

        [JsonPropertyName("isCommercialFree")]
        public bool? IsCommercialFree { get; set; }

        [JsonPropertyName("stationLogo")]
        //[JsonConverter(typeof(SingleOrListConverter<StationImage>))]
        public List<StationImage> StationLogos { get; set; }

        [JsonPropertyName("logo")]
        public StationImage Logo { get; set; }
    }

    public class StationImage
    {
        [JsonPropertyName("URL")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }

    public class StationBroadcaster
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postalcode")]
        public string Postalcode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }

    public class LineupMetadata
    {
        [JsonPropertyName("lineup")]
        public string Lineup { get; set; }

        [JsonPropertyName("modified")]
        public string Modified { get; set; }

        [JsonPropertyName("transport")]
        public string Transport { get; set; }

        [JsonPropertyName("modulation")]
        public string Modulation { get; set; }
    }
}