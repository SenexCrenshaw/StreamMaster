//namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
//{
//    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
//    public class StationChannelMap
//    {
//        [JsonIgnore]
//        [IgnoreMember]
//        public Guid Id { get; set; } = Guid.NewGuid();

//        [JsonPropertyName("map")]
//        //[JsonConverter(typeof(SingleOrListConverter<LineupChannel>))]
//        public List<LineupChannelStation> Map { get; set; } = [];

//        [JsonPropertyName("stations")]
//        //[JsonConverter(typeof(SingleOrListConverter<LineupStation>))]
//        public List<LineupStation> Stations { get; set; } = [];

//        [JsonPropertyName("metadata")]
//        public LineupMetadata Metadata { get; set; } = new();
//    }
//}