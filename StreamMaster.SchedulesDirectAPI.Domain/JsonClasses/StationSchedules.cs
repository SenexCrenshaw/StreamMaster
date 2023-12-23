using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class ScheduleRequest
    {
        [JsonPropertyName("stationID")]
        public string StationId { get; set; }

        [JsonPropertyName("date")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Date { get; set; }
    }

    public class ScheduleMd5Response : BaseResponse
    {
        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }
    }

    public class ScheduleResponse : BaseResponse
    {
        [JsonPropertyName("stationID")]
        public string StationId { get; set; }

        [JsonPropertyName("retryTime")]
        public string RetryTime { get; set; }

        [JsonPropertyName("minDate")]
        public string MinDate { get; set; }

        [JsonPropertyName("maxDate")]
        public string MaxDate { get; set; }

        [JsonPropertyName("requestedDate")]
        public string RequestedDate { get; set; }

        [JsonPropertyName("programs")]
        //[JsonConverter(typeof(SingleOrListConverter<ScheduleProgram>))]
        public List<ScheduleProgram> Programs { get; set; }

        [JsonPropertyName("metadata")]
        public ScheduleMetadata Metadata { get; set; }
    }

    public class ScheduleProgram
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; }

        [JsonPropertyName("airDateTime")]
        public DateTime AirDateTime { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        /// <summary>
        /// Applied to any show/episode (including all episodes of a miniseries) indicated as “New” by the provider the first time it airs per country.
        /// </summary>
        [JsonPropertyName("new")]
        public bool New { get; set; }

        /// <summary>
        /// Educational content provided for classroom use.
        /// </summary>
        [JsonPropertyName("cableInTheClassroom")]
        public bool CableInTheClassroom { get; set; }

        /// <summary>
        /// Program is available after air via an on demand catchup service (VOD or OTT).
        /// </summary>
        [JsonPropertyName("catchup")]
        public bool Catchup { get; set; }

        /// <summary>
        /// Identifies a continuation of a program listed in the previous time slot on schedule.
        /// </summary>
        [JsonPropertyName("continued")]
        public bool Continued { get; set; }

        /// <summary>
        /// "Educational and informational" (or "educational and informative"), refers to a type of children's television programming shown in the United States.
        /// </summary>
        [JsonPropertyName("educational")]
        public bool Educational { get; set; }

        /// <summary>
        /// Broadcast of this program/movie on a channel will be joined after the originating station has already begun the telecast.
        /// </summary>
        [JsonPropertyName("joinedInProgress")]
        public bool JoinedInProgress { get; set; }

        /// <summary>
        /// Broadcast of this program/movie on a channel will terminate before the originating station finishes the telecast.
        /// </summary>
        [JsonPropertyName("leftInProgress")]
        public bool LeftInProgress { get; set; }

        /// <summary>
        /// Applied to movies and the first episode of a miniseries where indicated as such on schedules by the provider. Could indicate a world premiere of a movie or a channel premiere.
        /// </summary>
        [JsonPropertyName("premiere")]
        public bool Premiere { get; set; }

        /// <summary>
        /// Same as Continued.
        /// </summary>
        [JsonPropertyName("programBreak")]
        public bool ProgramBreak { get; set; }

        /// <summary>
        /// Identifies the second or later airing of a sporting event when a station is broadcasting a game more than once.
        /// </summary>
        [JsonPropertyName("repeat")]
        public bool Repeat { get; set; }

        /// <summary>
        /// Program is translated into sign language by an on-screen interpreter.
        /// </summary>
        [JsonPropertyName("signed")]
        public bool Signed { get; set; }

        /// <summary>
        /// Sport event may be blacked out in certain markets based on specific agreements between networks and sports leagues.
        /// </summary>
        [JsonPropertyName("subjectToBlackout")]
        public bool SubjectToBlackout { get; set; }

        /// <summary>
        /// Indicates that the start time of a program may change, usually due to a live event taking place immediately before it on a schedule.
        /// </summary>
        [JsonPropertyName("timeApproximate")]
        public bool TimeApproximate { get; set; }

        /// <summary>
        /// Live := Program is being broadcast live, as indicated by the provider.
        /// Tape := Identifies the first airing of a sporting event that took place on a prior calendar day.
        /// Delay := Identifies the first airing of a sporting event that took place early on the same schedule day.
        /// </summary>
        [JsonPropertyName("liveTapeDelay")]
        public string LiveTapeDelay { get; set; }

        /// <summary>
        /// Season Finale := Indicates the initial airing of final episode of a season of a TV series.
        /// Season Premiere := Indicates the initial airing of the first episode of a season of a TV series.
        /// Series Finale := Indicates the initial airing of the final episode of the final season of a TV series.
        /// Series Premiere := Indicates the initial airing of the first episode of the first season of a season of a TV series.
        /// </summary>
        [JsonPropertyName("isPremiereOrFinale")]
        public string IsPremiereOrFinale { get; set; }

        /// <summary>
        /// cc := Program is encoded with captioning of dialogue for the hearing impaired.
        /// DD := Programs is available in Dolby Digital sound.
        /// DD 5.1 := Program is available in Dolby Digital 5.1 digital sound
        /// Dolby := Program is available in some form of Dolby digital sound
        /// DVS := "Descriptive Video Service" - Program is available with an audio descriptive feed (audio narration of what is taking place on the screen).
        /// stereo := Program is available in some form of stereo sound.
        /// </summary>
        [JsonPropertyName("audioProperties")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] AudioProperties { get; set; }

        /// <summary>
        /// HDTV := Program has been identified by the provider on a schedule as being available in a high-definition format. Could be either native HD or upconverted from SDTV.
        /// HD 1080i, HD 1080p, HD 720p, HD 480p
        /// HD Unknown := The last level (HD Unknown) indicates specific program/event manually marked as unknown level.
        /// Letterbox := Program is telecast in a Letterbox/Widescreen format.
        /// UHDTV := This qualifier indicates that the program has been identified by the provider on a schedule as being available in Ultra high-definition format.
        /// HDR := HDR is the generic label that Gracenote is using to identify any brand of HDR. All HDR content is UHD (4K),  but NOT all 4K is HDR.
        /// Widescreen := Same as Letterbox.
        /// </summary>
        [JsonPropertyName("videoProperties")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] VideoProperties { get; set; }

        /// <summary>
        /// Programs for which provider suggests parental advisory based on program content.
        /// </summary>
        [JsonPropertyName("ratings")]
        //[JsonConverter(typeof(SingleOrListConverter<ScheduleTvRating>))]
        public List<ScheduleTvRating> Ratings { get; set; }

        [JsonPropertyName("multipart")]
        public ScheduleMultipart Multipart { get; set; }
    }

    public class ScheduleMetadata
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("modified")]
        public string Modified { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }
    }

    public class ScheduleMultipart
    {
        [JsonPropertyName("partNumber")]
        public int PartNumber { get; set; }

        [JsonPropertyName("totalParts")]
        public int TotalParts { get; set; }
    }

    public class ScheduleTvRating
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}