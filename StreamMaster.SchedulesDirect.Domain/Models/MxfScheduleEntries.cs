using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models
{
    public class MxfScheduleEntries
    {
        [XmlAttribute("service")]
        public string Service { get; set; } = string.Empty;

        [XmlElement("ScheduleEntry")]
        public List<MxfScheduleEntry> ScheduleEntry { get; set; } = [];
        public bool ShouldSerializeScheduleEntry()
        {
            ScheduleEntry = [.. ScheduleEntry.OrderBy(arg => arg.StartTime)];
            DateTime endTime = DateTime.MinValue;
            foreach (MxfScheduleEntry entry in ScheduleEntry)
            {
                if (entry.StartTime != endTime)
                {
                    entry.IncludeStartTime = true;
                }

                endTime = entry.StartTime + TimeSpan.FromSeconds(entry.Duration);
            }
            return true;
        }
    }

    public class MxfScheduleEntry
    {
        private int _tvRating;

        [XmlIgnore] public MxfProgram mxfProgram = new();
        //[XmlIgnore] public XmltvProgramme? XmltvProgramme { get; set; }
        [XmlIgnore] public bool IncludeStartTime;
        [XmlIgnore] public Dictionary<string, dynamic> extras = [];

        // duplicate of Microsoft.MediaCenter.Guide.TVRating enum
        private enum McepgTvRating
        {
            Unknown = 0,
            UsaY = 1,
            UsaY7 = 2,
            UsaG = 3,
            UsaPg = 4,
            UsaTV14 = 5,
            UsaMA = 6,
            DeAll = 7,
            De6 = 8,
            De12 = 9,
            De16 = 10,
            DeAdults = 11,
            FrAll = 12,
            Fr10 = 13,
            Fr12 = 14,
            Fr16 = 15,
            Fr18 = 16,
            KrAll = 17,
            Kr7 = 18,
            Kr12 = 19,
            Kr15 = 20,
            Kr19 = 21,
            GB_UC = 22,
            GbU = 23,
            GbPG = 24,
            Gb12 = 25,
            Gb15 = 26,
            Gb18 = 27,
            GbR18 = 28
        }

        /// <summary>
        /// An ID of a Program element.
        /// </summary>
        [XmlAttribute("program")]
        [DefaultValue(0)]
        public int Program { get; set; }

        /// <summary>
        /// Specifies the start time of the broadcast.
        /// The dateTime type is in UTC. This attribute is only specified for the first ScheduleEntry element in a group.
        /// </summary>
        [XmlAttribute("startTime")]
        public DateTime StartTime { get; set; }
        public bool ShouldSerializeStartTime() { return IncludeStartTime; }

        /// <summary>
        /// The duration of the broadcast, in seconds.
        /// </summary>
        [XmlAttribute("duration")]
        [DefaultValue(0)]
        public int Duration { get; set; }

        /// <summary>
        /// Indicates whether this broadcast is closed captioned.
        /// </summary>
        [XmlAttribute("isCC")]
        [DefaultValue(false)]
        public bool IsCc { get; set; }

        /// <summary>
        /// Indicates whether this broadcast is deaf-signed
        /// </summary>
        [XmlAttribute("isSigned")]
        [DefaultValue(false)]
        public bool IsSigned { get; set; }

        /// <summary>
        /// Indicates the audio format of the broadcast.
        /// Possible values are:
        /// 0 = Not specified
        /// 1 = Mono
        /// 2 = Stereo
        /// 3 = Dolby
        /// 4 = Dolby Digital
        /// 5 = THX
        /// </summary>
        [XmlAttribute("audioFormat")]
        [DefaultValue(0)]
        public int AudioFormat { get; set; }

        /// <summary>
        /// Indicates whether this is a live broadcast.
        /// </summary>
        [XmlAttribute("isLive")]
        [DefaultValue(false)]
        public bool IsLive { get; set; }

        /// <summary>
        /// Indicates whether this is live sports event.
        /// </summary>
        [XmlAttribute("isLiveSports")]
        [DefaultValue(false)]
        public bool IsLiveSports { get; set; }

        /// <summary>
        /// Indicates whether this program has been taped and is being replayed (for example, a sports event).
        /// </summary>
        [XmlAttribute("isTape")]
        [DefaultValue(false)]
        public bool IsTape { get; set; }

        /// <summary>
        /// Indicates whether this program is being broadcast delayed (for example, an award show such as the Academy Awards).
        /// </summary>
        [XmlAttribute("isDelay")]
        [DefaultValue(false)]
        public bool IsDelay { get; set; }

        /// <summary>
        /// Indicates whether this program is subtitled.
        /// </summary>
        [XmlAttribute("isSubtitled")]
        [DefaultValue(false)]
        public bool IsSubtitled { get; set; }

        /// <summary>
        /// Indicates whether this program is a premiere.
        /// </summary>
        [XmlAttribute("isPremiere")]
        [DefaultValue(false)]
        public bool IsPremiere { get; set; }

        /// <summary>
        /// Indicates whether this program is a finale.
        /// </summary>
        [XmlAttribute("isFinale")]
        [DefaultValue(false)]
        public bool IsFinale { get; set; }

        /// <summary>
        /// Indicates whether this program was joined in progress.
        /// </summary>
        [XmlAttribute("isInProgress")]
        [DefaultValue(false)]
        public bool IsInProgress { get; set; }

        /// <summary>
        /// Indicates whether this program has a secondary audio program broadcast at the same time.
        /// </summary>
        [XmlAttribute("isSap")]
        [DefaultValue(false)]
        public bool IsSap { get; set; }

        /// <summary>
        /// Indicates whether this program has been blacked out.
        /// </summary>
        [XmlAttribute("isBlackout")]
        [DefaultValue(false)]
        public bool IsBlackout { get; set; }

        /// <summary>
        /// Indicates whether this program has been broadcast with an enhanced picture.
        /// </summary>
        [XmlAttribute("isEnhanced")]
        [DefaultValue(false)]
        public bool IsEnhanced { get; set; }

        /// <summary>
        /// Indicates whether this program is broadcast in 3D.
        /// </summary>
        [XmlAttribute("is3D")]
        [DefaultValue(false)]
        public bool Is3D { get; set; }

        /// <summary>
        /// Indicates whether this program is broadcast in letterbox format.
        /// </summary>
        [XmlAttribute("isLetterbox")]
        [DefaultValue(false)]
        public bool IsLetterbox { get; set; }

        /// <summary>
        /// Indicates whether this program is broadcast in high definition (HD).
        /// Determines whether the HD icon is displayed.
        /// </summary>
        [XmlAttribute("isHdtv")]
        [DefaultValue(false)]
        public bool IsHdtv { get; set; }

        /// <summary>
        /// Indicates whether this program is broadcast simultaneously in HD.
        /// </summary>
        [XmlAttribute("isHdtvSimulCast")]
        [DefaultValue(false)]
        public bool IsHdtvSimulCast { get; set; }

        /// <summary>
        /// Indicates whether this program is broadcast with Descriptive Video Service (DVS).
        /// </summary>
        [XmlAttribute("isDvs")]
        [DefaultValue(false)]
        public bool IsDvs { get; set; }

        /// <summary>
        /// Specifies the part number (for instance, if this is part 1 of 3, use "1").
        /// </summary>
        [XmlAttribute("part")]
        [DefaultValue(0)]
        public int Part { get; set; }

        /// <summary>
        /// Specifies the total number of parts (for instance, if this is part 1 of 3, use "3").
        /// </summary>
        [XmlAttribute("parts")]
        [DefaultValue(0)]
        public int Parts { get; set; }

        /// <summary>
        /// Specifies the TV parental rating (not documented on website)
        /// </summary>
        [XmlAttribute("tvRating")]
        [DefaultValue(0)]
        public int TvRating
        {
            get
            {
                if (_tvRating > 0)
                {
                    return _tvRating;
                }

                Dictionary<string, string> ratings = [];
                if (extras.TryGetValue("ratings", out dynamic? value))
                {
                    foreach (KeyValuePair<string, string> rating in value)
                    {
                        if (!ratings.TryGetValue(rating.Key, out _))
                        {
                            ratings.Add(rating.Key, rating.Value);
                        }
                    }
                }

                //if (mxfProgram?.Extras.ContainsKey("ratings") ?? false)
                //{
                //    foreach (KeyValuePair<string, string> rating in mxfProgram.Extras["ratings"])
                //    {
                //        if (!ratings.TryGetValue(rating.Key, out _))
                //        {
                //            ratings.Add(rating.Key, rating.Value);
                //        }
                //    }
                //}

                int maxValue = 0;
                foreach (KeyValuePair<string, string> keyValue in ratings)
                {
                    switch (keyValue.Key)
                    {
                        case "USA Parental Rating":
                            switch (keyValue.Value.ToLower())
                            {
                                // USA Parental Rating
                                case "tvy":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaY);
                                    break;
                                case "tvy7":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaY7);
                                    break;
                                case "tvg":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaG);
                                    break;
                                case "tvpg":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaPg);
                                    break;
                                case "tv14":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaTV14);
                                    break;
                                case "tvma":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.UsaMA);
                                    break;
                            }
                            break;
                        case "Freiwillige Selbstkontrolle der Filmwirtschaft":
                            switch (keyValue.Value.ToLower())
                            {
                                // DEU Freiwillige Selbstkontrolle der Filmwirtschaft
                                case "0":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.DeAll);
                                    break;
                                case "6":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.De6);
                                    break;
                                case "12":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.De12);
                                    break;
                                case "16":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.De16);
                                    break;
                                case "18":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.DeAdults);
                                    break;
                            }
                            break;
                        case "Conseil Supérieur de l'Audiovisuel":
                            switch (keyValue.Value.ToLower())
                            {
                                // FRA Conseil Supérieur de l'Audiovisuel
                                //case "":
                                //    maxValue = Math.Max(maxValue, (int)McepgTVRating.FrAll);
                                //    break;
                                case "-10":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Fr10);
                                    break;
                                case "-12":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Fr12);
                                    break;
                                case "-16":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Fr16);
                                    break;
                                case "-18":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Fr18);
                                    break;
                            }
                            break;
                        case "UK Content Provider":
                        case "British Board of Film Classification":
                            switch (keyValue.Value.ToLower())
                            {
                                // GBR UK Content Provider
                                // GBR British Board of Film Classification
                                case "uc":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.GB_UC);
                                    break;
                                case "u":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.GbU);
                                    break;
                                case "pg":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.GbPG);
                                    break;
                                case "12":
                                case "12a":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Gb12);
                                    break;
                                case "15":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Gb15);
                                    break;
                                case "18":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.Gb18);
                                    break;
                                case "r18":
                                    maxValue = Math.Max(maxValue, (int)McepgTvRating.GbR18);
                                    break;
                            }
                            break;
                    }
                }
                return maxValue;
            }
            set => _tvRating = value;
        }

        [XmlAttribute("isClassroom")]
        [DefaultValue(false)]
        public bool IsClassroom { get; set; }

        [XmlAttribute("isRepeat")]
        [DefaultValue(false)]
        public bool IsRepeat { get; set; }
    }
}
