using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramEventDetails
    {
        [JsonPropertyName("venue100")]
        public string Venue100 { get; set; } = string.Empty;

        [JsonPropertyName("teams")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramEventDetailsTeam>))]
        public List<ProgramEventDetailsTeam> Teams { get; set; } = [];

        [JsonPropertyName("gameDate")]
        public DateTime GameDate { get; set; } = DateTime.MinValue;
        public bool ShouldSerializeGameDate()
        {
            return GameDate.Ticks > 0;
        }
    }
}