using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class AddRemoveLineupResponse : BaseResponse
    {
        [JsonPropertyName("changesRemaining")]
        public int ChangesRemaining { get; set; }
    }
}