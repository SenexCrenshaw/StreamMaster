using System.Globalization;

using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

using StreamMaster.Domain.Helpers;

namespace StreamMaster.SchedulesDirect.Converters
{
    public class XmltvProgramBuilder(IOptionsMonitor<SDSettings> sdSettingsMonitor, ISchedulesDirectDataService schedulesDirectDataService)
        : IXmltvProgramBuilder
    {
        private static readonly string[] TvRatings =
        [
            "", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA",
            "", "Kinder bis 12 Jahren", "Freigabe ab 12 Jahren", "Freigabe ab 16 Jahren", "Keine Jugendfreigabe",
            "", "Déconseillé aux moins de 10 ans", "Déconseillé aux moins de 12 ans", "Déconseillé aux moins de 16 ans", "Déconseillé aux moins de 18 ans",
            "모든 연령 시청가", "7세 이상 시청가", "12세 이상 시청가", "15세 이상 시청가", "19세 이상 시청가",
            "SKY-UC", "SKY-U", "SKY-PG", "SKY-12", "SKY-15", "SKY-18", "SKY-R18"
        ];

        private static readonly string[] MpaaRatings =
        [
            "", "G", "PG", "PG-13", "R", "NC-17", "X", "NR", "AO"
        ];

        public XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId, int timeShift, string baseUrl)
        {
            MxfProgram mxfProgram = scheduleEntry.mxfProgram;
            SDSettings sdSettings = sdSettingsMonitor.CurrentValue;

            DateTime startDT = scheduleEntry.StartTime.AddMinutes(timeShift);

            XmltvProgramme programme = new()
            {
                Start = FormatDateTime(startDT),
                Stop = FormatDateTime(startDT.AddSeconds(scheduleEntry.Duration)),
                Channel = channelId,
                Titles = ConvertToXmltvTextList(mxfProgram.Title),
                SubTitles = ConvertToXmltvTextList(mxfProgram.EpisodeTitle),
                Descriptions = ConvertToXmltvTextList(GetFullDescription(mxfProgram, scheduleEntry, sdSettings)),
                Credits = BuildProgramCredits(mxfProgram),
                Date = BuildProgramDate(mxfProgram),
                //Categories = BuildProgramCategories(mxfProgram),
                Language = ConvertToXmltvText(mxfProgram.Language),
                Icons = BuildProgramIcons(scheduleEntry, baseUrl),
                EpisodeNums = BuildEpisodeNumbers(scheduleEntry),
                Video = BuildProgramVideo(scheduleEntry),
                Audio = BuildProgramAudio(scheduleEntry),
                PreviouslyShown = BuildProgramPreviouslyShown(scheduleEntry),
                Premiere = BuildProgramPremiere(scheduleEntry),
                Live = scheduleEntry.IsLive ? string.Empty : null,
                New = !scheduleEntry.IsRepeat ? string.Empty : null,
                SubTitles2 = BuildProgramSubtitles(scheduleEntry),
                Rating = BuildProgramRatings(scheduleEntry),
                StarRating = BuildProgramStarRatings(mxfProgram)
            };

            if (programme.Icons is not null)
            {
                int aa = 1;
            }

            return programme;
        }

        private static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss +0000", CultureInfo.InvariantCulture);
        }

        private static List<XmltvText>? ConvertToXmltvTextList(string? text)
        {
            return string.IsNullOrEmpty(text) ? null : [new XmltvText { Text = text }];
        }

        private static XmltvText? ConvertToXmltvText(string? text)
        {
            return string.IsNullOrEmpty(text) ? null : new XmltvText { Text = text };
        }

        private static string GetFullDescription(MxfProgram mxfProgram, MxfScheduleEntry scheduleEntry, SDSettings sdSettings)
        {
            string extendedDescription = sdSettings.XmltvExtendedInfoInTitleDescriptions
                ? GetExtendedDescription(mxfProgram, scheduleEntry, sdSettings)
                : string.Empty;

            return (extendedDescription + mxfProgram.Description).Trim();
        }

        private static string GetExtendedDescription(MxfProgram mxfProgram, MxfScheduleEntry scheduleEntry, SDSettings sdSettings)
        {
            string descriptionExtended = string.Empty;

            if (mxfProgram.IsMovie && mxfProgram.Year > 0)
            {
                descriptionExtended = $"{mxfProgram.Year}";
            }
            else if (!mxfProgram.IsMovie)
            {
                if (scheduleEntry.IsLive)
                {
                    descriptionExtended = "[LIVE]";
                }
                else if (scheduleEntry.IsPremiere)
                {
                    descriptionExtended = "[PREMIERE]";
                }
                else if (scheduleEntry.IsFinale)
                {
                    descriptionExtended = "[FINALE]";
                }
                else if (!scheduleEntry.IsRepeat)
                {
                    descriptionExtended = "[NEW]";
                }
                else if (scheduleEntry.IsRepeat && !mxfProgram.IsGeneric)
                {
                    descriptionExtended = "[REPEAT]";
                }

                if (!sdSettings.PrefixEpisodeTitle && !sdSettings.PrefixEpisodeDescription && !sdSettings.AppendEpisodeDesc)
                {
                    if (mxfProgram.SeasonNumber > 0 && mxfProgram.EpisodeNumber > 0)
                    {
                        descriptionExtended += $" S{mxfProgram.SeasonNumber}:E{mxfProgram.EpisodeNumber}";
                    }
                    else if (mxfProgram.EpisodeNumber > 0)
                    {
                        descriptionExtended += $" #{mxfProgram.EpisodeNumber}";
                    }
                }
            }

            if (!string.IsNullOrEmpty(TvRatings[scheduleEntry.TvRating]))
            {
                descriptionExtended += $" {TvRatings[scheduleEntry.TvRating]}";
                if (mxfProgram.MpaaRating > 0)
                {
                    descriptionExtended += ",";
                }
            }
            if (mxfProgram.MpaaRating > 0)
            {
                descriptionExtended += $" {MpaaRatings[mxfProgram.MpaaRating]}";
            }

            string advisories = GetAdvisories(mxfProgram);

            if (!string.IsNullOrEmpty(advisories))
            {
                descriptionExtended += $" ({advisories.Trim().TrimEnd(',').Replace(",", ", ")})";
            }

            if (mxfProgram.IsMovie && mxfProgram.HalfStars > 0)
            {
                descriptionExtended += $" {mxfProgram.HalfStars * 0.5:N1}/4.0";
            }
            else if (!mxfProgram.IsMovie)
            {
                if (!mxfProgram.IsGeneric && !string.IsNullOrEmpty(mxfProgram.OriginalAirdate))
                {
                    descriptionExtended += $" Original air date: {DateTime.Parse(mxfProgram.OriginalAirdate):d}";
                }
            }

            if (!string.IsNullOrEmpty(descriptionExtended))
            {
                descriptionExtended = descriptionExtended.Trim() + "\r\n";
            }

            return descriptionExtended;
        }

        private static string GetAdvisories(MxfProgram mxfProgram)
        {
            string advisories = string.Empty;
            if (mxfProgram.HasAdult)
            {
                advisories += "Adult Situations,";
            }

            if (mxfProgram.HasGraphicLanguage)
            {
                advisories += "Graphic Language,";
            }
            else if (mxfProgram.HasLanguage)
            {
                advisories += "Language,";
            }

            if (mxfProgram.HasStrongSexualContent)
            {
                advisories += "Strong Sexual Content,";
            }

            if (mxfProgram.HasGraphicViolence)
            {
                advisories += "Graphic Violence,";
            }
            else if (mxfProgram.HasMildViolence)
            {
                advisories += "Mild Violence,";
            }
            else if (mxfProgram.HasViolence)
            {
                advisories += "Violence,";
            }

            if (mxfProgram.HasNudity)
            {
                advisories += "Nudity,";
            }
            else if (mxfProgram.HasBriefNudity)
            {
                advisories += "Brief Nudity,";
            }

            if (mxfProgram.HasRape)
            {
                advisories += "Rape,";
            }

            return advisories;
        }

        private static XmltvCredit? BuildProgramCredits(MxfProgram mxfProgram)
        {
            bool hasCredits = mxfProgram.DirectorRole?.Count > 0 ||
                              mxfProgram.ActorRole?.Count > 0 ||
                              mxfProgram.WriterRole?.Count > 0 ||
                              mxfProgram.ProducerRole?.Count > 0 ||
                              mxfProgram.HostRole?.Count > 0 ||
                              mxfProgram.GuestActorRole?.Count > 0;

            return !hasCredits
                ? null
                : new XmltvCredit
                {
                    Directors = MxfPersonRankToXmltvCrew(mxfProgram.DirectorRole),
                    Actors = MxfPersonRankToXmltvActors(mxfProgram.ActorRole),
                    Writers = MxfPersonRankToXmltvCrew(mxfProgram.WriterRole),
                    Producers = MxfPersonRankToXmltvCrew(mxfProgram.ProducerRole),
                    Presenters = MxfPersonRankToXmltvCrew(mxfProgram.HostRole),
                    Guests = MxfPersonRankToXmltvCrew(mxfProgram.GuestActorRole)
                };
        }

        private static List<string>? MxfPersonRankToXmltvCrew(List<MxfPersonRank>? mxfPersons)
        {
            return mxfPersons?.Where(person => person.Name is not null).Select(person => person.Name!).ToList();
        }

        private static List<XmltvActor>? MxfPersonRankToXmltvActors(List<MxfPersonRank>? mxfPersons)
        {
            return mxfPersons?.Where(a => a.Name is not null).Select(person => new XmltvActor { Actor = person.Name!, Role = person.Character }).ToList();
        }

        private static string? BuildProgramDate(MxfProgram mxfProgram)
        {
            return mxfProgram.IsMovie && mxfProgram.Year > 0
                ? mxfProgram.Year.ToString()
                : !string.IsNullOrEmpty(mxfProgram.OriginalAirdate) ? DateTime.Parse(mxfProgram.OriginalAirdate).ToString("yyyyMMdd") : null;
        }

        //private List<XmltvText>? BuildProgramCategories(MxfProgram mxfProgram)
        //{
        //    if (string.IsNullOrEmpty(mxfProgram.Keywords))
        //    {
        //        return null;
        //    }

        //    List<string> categories = [];

        //    foreach (string keywordId in mxfProgram.Keywords.Split(','))
        //    {
        //        if (keywordDict.TryGetValue(keywordId, out string? word))
        //        {
        //            categories.Add(word);
        //        }
        //    }

        //    if (categories.Remove("Movies"))
        //    {
        //        if (!categories.Contains("Movie"))
        //        {
        //            categories.Add("Movie");
        //        }
        //    }

        //    // Remove "Kids" if "Children" is also present
        //    if (categories.Contains("Children"))
        //    {
        //        _ = categories.Remove("Kids");
        //    }

        //    return categories.Count == 0 ? null : categories.ConvertAll(category => new XmltvText { Text = category });
        //}

        private List<XmltvIcon>? BuildProgramIcons(MxfScheduleEntry scheduleEntry, string baseUrl)
        {
            SDSettings sdSettings = sdSettingsMonitor.CurrentValue;
            //MxfProgram mxfProgram = scheduleEntry.mxfProgram;
            ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData;

            if (scheduleEntry.mxfProgram.ProgramId.Equals(value: "EP019254150003"))
            {
                int aaa = 1;
            }

            MxfProgram? mxfProgram = schedulesDirectData.FindProgram(scheduleEntry.mxfProgram.ProgramId);

            if (mxfProgram == null)
            {
                return null;
            }

            if (mxfProgram.mxfSeason?.ArtWorks.Count > 0)
            {
                int aa = 1;
            }
            if (mxfProgram.mxfSeriesInfo?.ArtWorks.Count > 0)
            {
                int aa = 1;
            }
            // Get artwork from program or its related entities
            //List<ProgramArtwork>? artWorks = mxfProgram.ArtWorks
            //    ?? mxfProgram.mxfSeason?.ArtWorks
            //    ?? mxfProgram.mxfSeriesInfo?.ArtWorks;

            if (mxfProgram.ArtWorks.Count == 0)
            {
                return null;
            }

            // Ensure baseUrl ends with '/'
            baseUrl = !string.IsNullOrEmpty(baseUrl) && !baseUrl.EndsWith('/')
                ? $"{baseUrl}/"
                : baseUrl;

            // Handle single image setting
            if (sdSettings.XmltvSingleImage)
            {
                ProgramArtwork? firstArtwork = mxfProgram.ArtWorks.FirstOrDefault();
                return firstArtwork is not null
                    ?
                    [
                new XmltvIcon
                {
                    Src = $"{baseUrl}{firstArtwork.Uri}",
                    Width = firstArtwork.Width ?? 0,
                    Height = firstArtwork.Height ?? 0
                }
                    ]
                    : null;
            }

            // Map all artworks to XmltvIcons
            return mxfProgram.ArtWorks
                .ConvertAll(art => new XmltvIcon
                {
                    Src = $"{baseUrl}{art.Uri}",
                    Width = art.Width ?? 0,
                    Height = art.Height ?? 0
                })
;
        }

        private static List<XmltvEpisodeNum>? BuildEpisodeNumbers(MxfScheduleEntry scheduleEntry)
        {
            List<XmltvEpisodeNum> list = [];
            MxfProgram mxfProgram = scheduleEntry.mxfProgram;

            if (mxfProgram.EpisodeNumber != 0 || scheduleEntry.Part != 0)
            {
                string seasonPart = mxfProgram.SeasonNumber > 0 ? (mxfProgram.SeasonNumber - 1).ToString() : "";
                string episodePart = mxfProgram.EpisodeNumber > 0 ? (mxfProgram.EpisodeNumber - 1).ToString() : "";
                string part = scheduleEntry.Part > 0 ? $"{scheduleEntry.Part - 1}/" : "0/";
                string parts = scheduleEntry.Parts > 0 ? scheduleEntry.Parts.ToString() : "1";
                string text = $"{seasonPart}.{episodePart}.{part}{parts}";

                list.Add(new XmltvEpisodeNum { System = "xmltv_ns", Text = text });
            }
            else if (mxfProgram.EPGNumber is EPGHelper.DummyId or EPGHelper.CustomPlayListId)
            {
                list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = $"{scheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd}" });
            }
            else if (!mxfProgram.ProgramId.StartsWith("MV"))
            {
                if (string.IsNullOrEmpty(mxfProgram.OriginalAirdate))
                {
                    list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = $"{scheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd}" });
                }
                else
                {
                    string oad = !scheduleEntry.IsRepeat
                        ? $"{scheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd}"
                        : $"{DateTime.Parse(mxfProgram.OriginalAirdate):yyyy-MM-dd}";

                    list.Add(new XmltvEpisodeNum
                    {
                        System = "original-air-date",
                        Text = oad
                    });
                }
            }

            //if (mxfProgram.Series != null)
            //{
            //    if (int.TryParse(mxfProgram.Series[2..], out int seriesIndex) &&
            //        seriesDict.TryGetValue(seriesIndex - 1, out SeriesInfo? mxfSeriesInfo) &&
            //        mxfSeriesInfo.Extras.TryGetValue("tvdb", out dynamic? value))
            //    {
            //        list.Add(new XmltvEpisodeNum { System = "thetvdb.com", Text = $"series/{value}" });
            //    }
            //}

            return list.Count > 0 ? list : null;
        }

        private static XmltvVideo? BuildProgramVideo(MxfScheduleEntry scheduleEntry)
        {
            return scheduleEntry.IsHdtv ? new XmltvVideo { Quality = "HDTV" } : null;
        }

        private static XmltvAudio? BuildProgramAudio(MxfScheduleEntry scheduleEntry)
        {
            if (scheduleEntry.AudioFormat <= 0)
            {
                return null;
            }

            string format = scheduleEntry.AudioFormat switch
            {
                1 => "mono",
                2 => "stereo",
                3 => "dolby",
                4 => "dolby digital",
                5 => "surround",
                _ => string.Empty
            };

            return !string.IsNullOrEmpty(format) ? new XmltvAudio { Stereo = format } : null;
        }

        private static XmltvPreviouslyShown? BuildProgramPreviouslyShown(MxfScheduleEntry scheduleEntry)
        {
            return scheduleEntry.IsRepeat && !scheduleEntry.mxfProgram.IsMovie ? new XmltvPreviouslyShown { Text = string.Empty } : null;
        }

        private static XmltvText? BuildProgramPremiere(MxfScheduleEntry scheduleEntry)
        {
            if (!scheduleEntry.IsPremiere)
            {
                return null;
            }

            MxfProgram mxfProgram = scheduleEntry.mxfProgram;
            string text = mxfProgram.IsMovie
                ? "Movie Premiere"
                : mxfProgram.IsSeriesPremiere ? "Series Premiere" : mxfProgram.IsSeasonPremiere ? "Season Premiere" : "Miniseries Premiere";
            return new XmltvText { Text = text };
        }

        private static List<XmltvSubtitles>? BuildProgramSubtitles(MxfScheduleEntry scheduleEntry)
        {
            List<XmltvSubtitles> list = [];

            if (scheduleEntry.IsCc)
            {
                list.Add(new XmltvSubtitles { Type = "teletext" });
            }
            if (scheduleEntry.IsSubtitled)
            {
                list.Add(new XmltvSubtitles { Type = "onscreen" });
            }
            if (scheduleEntry.IsSigned)
            {
                list.Add(new XmltvSubtitles { Type = "deaf-signed" });
            }

            return list.Count > 0 ? list : null;
        }

        private static List<XmltvRating> BuildProgramRatings(MxfScheduleEntry scheduleEntry)
        {
            List<XmltvRating> list = [];
            MxfProgram mxfProgram = scheduleEntry.mxfProgram;

            // Add advisories
            AddProgramRatingAdvisory(mxfProgram.HasAdult, list, "Adult Situations");
            AddProgramRatingAdvisory(mxfProgram.HasBriefNudity, list, "Brief Nudity");
            AddProgramRatingAdvisory(mxfProgram.HasGraphicLanguage, list, "Graphic Language");
            AddProgramRatingAdvisory(mxfProgram.HasGraphicViolence, list, "Graphic Violence");
            AddProgramRatingAdvisory(mxfProgram.HasLanguage, list, "Language");
            AddProgramRatingAdvisory(mxfProgram.HasMildViolence, list, "Mild Violence");
            AddProgramRatingAdvisory(mxfProgram.HasNudity, list, "Nudity");
            AddProgramRatingAdvisory(mxfProgram.HasRape, list, "Rape");
            AddProgramRatingAdvisory(mxfProgram.HasStrongSexualContent, list, "Strong Sexual Content");
            AddProgramRatingAdvisory(mxfProgram.HasViolence, list, "Violence");

            // Add ratings
            AddProgramRating(scheduleEntry, list);

            return list;
        }

        private static void AddProgramRatingAdvisory(bool hasAdvisory, List<XmltvRating> list, string advisory)
        {
            if (hasAdvisory)
            {
                list.Add(new XmltvRating { System = "advisory", Value = advisory });
            }
        }

        private static void AddProgramRating(MxfScheduleEntry scheduleEntry, List<XmltvRating> list)
        {
            HashSet<string> ratingsSet = [];

            if (scheduleEntry.extras.TryGetValue("ratings", out dynamic? value))
            {
                foreach (KeyValuePair<string, string> rating in (Dictionary<string, string>)value)
                {
                    if (ratingsSet.Add(rating.Key))
                    {
                        list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
                    }
                }
            }

            if (scheduleEntry.mxfProgram.Extras.TryGetValue("ratings", out dynamic? valueRatings))
            {
                foreach (KeyValuePair<string, string> rating in (Dictionary<string, string>)valueRatings)
                {
                    if (ratingsSet.Add(rating.Key))
                    {
                        list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
                    }
                }
            }

            if (scheduleEntry.TvRating != 0)
            {
                string rating = TvRatings[scheduleEntry.TvRating];
                if (!string.IsNullOrEmpty(rating))
                {
                    list.Add(new XmltvRating { System = "VCHIP", Value = rating });
                }
            }
        }

        private static List<XmltvRating>? BuildProgramStarRatings(MxfProgram mxfProgram)
        {
            return mxfProgram.HalfStars == 0
                ? null
                :
                [
                    new XmltvRating { Value = $"{mxfProgram.HalfStars * 0.5:N1}/4" }
                ];
        }
    }
}