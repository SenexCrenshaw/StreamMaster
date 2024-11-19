using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramDescriptions
    {
        [JsonPropertyName("description100")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramDescription>))]
        public List<ProgramDescription> Description100 { get; set; } = [];

        [JsonPropertyName("description1000")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramDescription>))]
        public List<ProgramDescription> Description1000 { get; set; } = [];
    }
}