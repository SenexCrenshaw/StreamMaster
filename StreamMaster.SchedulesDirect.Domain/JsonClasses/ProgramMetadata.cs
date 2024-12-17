using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramMetadata : BaseResponse
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        [JsonConverter(typeof(SingleOrListConverter<ProgramArtwork>))]
        public List<ProgramArtwork> Data { get; set; } = [];
    }
}