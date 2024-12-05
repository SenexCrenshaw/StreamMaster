//using System.Text.Json.Serialization;

//namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
//{
//    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//    public class LineupStation
//    {
//        [JsonPropertyName("stationID")]
//        public string StationId { get; set; } = string.Empty;

//        [JsonPropertyName("name")]
//        public string Name { get; set; } = string.Empty;

//        [JsonPropertyName("callsign")]
//        public string Callsign { get; set; } = string.Empty;

//        [JsonPropertyName("affiliate")]
//        public string Affiliate { get; set; } = string.Empty;

//        [JsonPropertyName("broadcastLanguage")]
//        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
//        public List<string> BroadcastLanguage { get; set; } = [];

//        [JsonPropertyName("descriptionLanguage")]
//        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
//        public List<string> DescriptionLanguage { get; set; } = [];

//        [JsonPropertyName("broadcaster")]
//        public StationBroadcaster Broadcaster { get; set; } = new();

//        [JsonPropertyName("isCommercialFree")]
//        public bool IsCommercialFree { get; set; }

//        [JsonPropertyName("stationLogo")]
//        //[JsonConverter(typeof(SingleOrListConverter<StationImage>))]
//        public List<StationImage> StationLogos { get; set; } = [];

//        [JsonPropertyName("logo")]
//        public StationImage Logo { get; set; } = new();
//    }
//}