using System.Text.Json.Serialization;

using System;
using System.Collections.Generic;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class UserStatus : BaseResponse
    {
        [JsonPropertyName("account")]
        public StatusAccount Account { get; set; }

        [JsonPropertyName("lineups")]
       // //[JsonConverter(typeof(SingleOrListConverter<StatusLineup>))]
        public List<StatusLineup> Lineups { get; set; }

        [JsonPropertyName("lastDataUpdate")]
        public DateTime LastDataUpdate { get; set; }

        [JsonPropertyName("notifications")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Notifications { get; set; }

        [JsonPropertyName("systemStatus")]
        //[JsonConverter(typeof(SingleOrListConverter<SystemStatus>))]
        public List<SystemStatus> SystemStatus { get; set; } = [];
    }

    public class StatusAccount
    {
        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }

        [JsonPropertyName("messages")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Messages { get; set; }

        [JsonPropertyName("maxLineups")]
        public int MaxLineups { get; set; }
    }

    public class StatusLineup
    {
        [JsonPropertyName("lineup")]
        public string Lineup { get; set; }

        [JsonPropertyName("modified")]
        public string Modified { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

    public class SystemStatus
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}