using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class AddRemoveLineupResponse : BaseResponse
    {
        [JsonPropertyName("changesRemaining")]
        public int ChangesRemaining { get; set; }
    }
}