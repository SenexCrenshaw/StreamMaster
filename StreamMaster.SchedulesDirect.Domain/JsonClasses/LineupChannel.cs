using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using MessagePack;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public partial class LineupChannel
    {
        public string ChannelNumber => $"{MyChannelNumber}{(myChannelSubnumber > 0 ? $".{myChannelSubnumber}" : "")}";
        public int MyChannelNumber
        {
            get
            {
                if (string.IsNullOrEmpty(Channel))
                {
                    return ChannelMajor ?? AtscMajor ?? UhfVhf ?? -1;
                }

                if (MyRegex().Match(Channel).Length > 0)
                {
                    return int.Parse(Channel[2..]);
                }

                if (MyRegex1().Match(Channel).Length > 0)
                {
                    return -1;
                }

                if (int.TryParse(MyRegex2().Replace(Channel, ""), out int number))
                {
                    return number;
                }
                else
                {
                    // if channel number is not a whole number, must be a decimal number
                    string[] numbers = MyRegex2().Replace(Channel, "").Replace('_', '.').Replace('-', '.').Split('.');
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
                if (string.IsNullOrEmpty(Channel))
                {
                    return ChannelMinor ?? AtscMinor ?? 0;
                }

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
        [IgnoreMember]
        public string MatchName { get; set; } = string.Empty;

        [JsonPropertyName("stationID")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("uhfVhf")]
        public int? UhfVhf { get; set; }

        [JsonPropertyName("atscMajor")]
        public int? AtscMajor { get; set; }

        [JsonPropertyName("atscMinor")]
        public int? AtscMinor { get; set; }

        [JsonPropertyName("atscType")]
        public string AtscType { get; set; } = string.Empty;

        [JsonPropertyName("frequencyHz")]
        public long FrequencyHz { get; set; }

        [JsonPropertyName("polarization")]
        public string Polarization { get; set; } = string.Empty;

        [JsonPropertyName("deliverySystem")]
        public string DeliverySystem { get; set; } = string.Empty;

        [JsonPropertyName("modulationSystem")]
        public string ModulationSystem { get; set; } = string.Empty;

        [JsonPropertyName("symbolrate")]
        public int Symbolrate { get; set; }

        [JsonPropertyName("fec")]
        public string Fec { get; set; } = string.Empty;

        [JsonPropertyName("serviceID")]
        public int ServiceId { get; set; }

        [JsonPropertyName("networkID")]
        public int NetworkId { get; set; }

        [JsonPropertyName("transportID")]
        public int TransportId { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("virtualChannel")]
        public string VirtualChannel { get; set; } = string.Empty;

        [JsonPropertyName("channelMajor")]
        public int? ChannelMajor { get; set; }

        [JsonPropertyName("channelMinor")]
        public int? ChannelMinor { get; set; }

        [JsonPropertyName("providerChannel")]
        public string ProviderChannel { get; set; } = string.Empty;

        [JsonPropertyName("providerCallsign")]
        public string ProviderCallsign { get; set; } = string.Empty;

        [JsonPropertyName("logicalChannelNumber")]
        public string LogicalChannelNumber { get; set; } = string.Empty;

        [JsonPropertyName("matchType")]
        public string MatchType { get; set; } = string.Empty;

        [GeneratedRegex(@"[A-Za-z]{1}[\d]{4}")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"[A-Za-z0-9.]\.[A-Za-z]{2}")]
        private static partial Regex MyRegex1();
        [GeneratedRegex("[^0-9.]")]
        private static partial Regex MyRegex2();
    }
}