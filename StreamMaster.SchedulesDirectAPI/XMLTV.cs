using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;

using System.Globalization;
using System.Net;

namespace StreamMaster.SchedulesDirectAPI;
public partial class SchedulesDirect
{
    private string _baseUrl = "";
    public XMLTV? CreateXmltv(string baseUrl,IEnumerable<string>? stationIds=null)
    {
        _baseUrl = baseUrl;
        try
        {
            var xmltv = new XMLTV
            {
                Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                SourceInfoUrl = "http://schedulesdirect.org",
                SourceInfoName = "Schedules Direct",
                GeneratorInfoName = "EPG123",
                GeneratorInfoUrl = "https://garyan2.github.io/",
                Channels = [],
                Programs = []
            };

            var settings = memoryCache.GetSetting();

            foreach (var service in schedulesDirectData.Services)
            {
                if (stationIds is not null && !stationIds.Contains(service.StationId))
                {
                    continue;
                }

                if (service.StationId == "DUMMY") continue;

                xmltv.Channels.Add(BuildXmltvChannel(service));

                if (service.MxfScheduleEntries.ScheduleEntry.Count == 0 && settings.SDSettings.XmltvAddFillerData)
                {
                    // add a program specific for this service
                    var program = schedulesDirectData.FindOrCreateProgram($"EPG123FILL{service.StationId}");
                    program.Title = service.Name;
                    program.Description = settings.SDSettings.XmltvFillerProgramDescription;
                    program.IsGeneric = true;

                    // populate the schedule entries
                    var startTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                    var stopTime = startTime + TimeSpan.FromDays(settings.SDSettings.SDEPGDays);
                    do
                    {
                        service.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
                        {
                            Duration = settings.SDSettings.XmltvFillerProgramLength * 60 * 60,
                            mxfProgram = program,
                            StartTime = startTime,
                            IsRepeat = true
                        });
                        startTime += TimeSpan.FromHours(settings.SDSettings.XmltvFillerProgramLength);
                    } while (startTime < stopTime);
                }

                foreach (var scheduleEntry in service.MxfScheduleEntries.ScheduleEntry)
                {
                    xmltv.Programs.Add(BuildXmltvProgram(scheduleEntry, $"EPG123.{service.StationId}.schedulesdirect.org"));
                }
            }
            return xmltv;
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Failed to create the XMLTV file. Exception:{FileUtil.ReportExceptionMessages(ex)}");
        }
        return null;
    }
    public string GetIconUrl(string iconOriginalSource, SMFileTypes? sMFileTypes=null)
    {
        var settings = memoryCache.GetSetting();
        if (string.IsNullOrEmpty(iconOriginalSource))
        {
            return $"{_baseUrl}{settings.DefaultIcon}";
        }

        string originalUrl = iconOriginalSource;

        if (iconOriginalSource.StartsWith('/'))
        {
            iconOriginalSource = iconOriginalSource[1..];
        }

        if (iconOriginalSource.StartsWith("images/"))
        {
            return $"{_baseUrl}/{iconOriginalSource}";
        }
        else if (!iconOriginalSource.StartsWith("http"))
        {
            return GetApiUrl(sMFileTypes??SMFileTypes.SDImage, originalUrl);
        }
        if (iconOriginalSource.StartsWith("https://json.schedulesdirect.org"))
        {
            return GetApiUrl(sMFileTypes ?? SMFileTypes.SDImage, originalUrl);
        }
        else if (settings.CacheIcons)
        {           
            return GetApiUrl(sMFileTypes ?? SMFileTypes.Icon, originalUrl);
        }

        return iconOriginalSource;
    }

    private string GetApiUrl(SMFileTypes path, string source)
    {
        return $"{_baseUrl}/api/files/{(int)path}/{WebUtility.UrlEncode(source)}";
    }

    #region ========== XMLTV Channels and Functions ==========
    public  XmltvChannel BuildXmltvChannel(MxfService mxfService)
    {
        var settings = memoryCache.GetSetting();

        // initialize the return channel
        var ret = new XmltvChannel
        {
            Id = $"EPG123.{mxfService.StationId}.schedulesdirect.org",
            DisplayNames = []
        };

        // minimum display names
        // 5MAXHD
        // 5 StarMAX HD East
        ret.DisplayNames.Add(new XmltvText { Text = mxfService.CallSign });
        if (!mxfService.Name.Equals(mxfService.CallSign))
        {
            ret.DisplayNames.Add(new XmltvText { Text = mxfService.Name });
        }

        // add channel number if requested
        if (settings.SDSettings.XmltvIncludeChannelNumbers)
        {
            var numbers = new HashSet<string>();
            foreach (var mxfLineup in schedulesDirectData.Lineups)
            {
                foreach (var mxfChannel in mxfLineup.channels)
                {
                    if (mxfChannel.Service != mxfService.Id || mxfChannel.Number <= 0) continue;

                    var num = $"{mxfChannel.Number}" + (mxfChannel.SubNumber > 0 ? $".{mxfChannel.SubNumber}" : "");
                    if (!numbers.Add(num)) continue;

                    ret.DisplayNames.Add(new XmltvText { Text = num + " " + mxfService.CallSign });
                    ret.DisplayNames.Add(new XmltvText { Text = num });
                }
            }
        }

        // add affiliate if present
        var affiliate = mxfService.mxfAffiliate?.Name;
        if (!string.IsNullOrEmpty(affiliate) && !mxfService.Name.Equals(affiliate)) ret.DisplayNames.Add(new XmltvText { Text = affiliate });

        // add logo if available
        if (mxfService.extras.TryGetValue("logo", out dynamic? logos))
        {
        
            ret.Icons =
                [
                    new() {
                        Src = GetIconUrl( mxfService.extras["logo"].Url),
                        Height = mxfService.extras["logo"].Height,
                        Width = mxfService.extras["logo"].Width
                    }
                ];

        }
        return ret;
    }
    #endregion

    #region ========== XMLTV Programmes and Functions ==========
    private  XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId)
    {
        var settings = memoryCache.GetSetting();
        var mxfProgram = scheduleEntry.mxfProgram;
        var descriptionExtended = string.Empty;
        if (!settings.SDSettings.XmltvExtendedInfoInTitleDescriptions || mxfProgram.IsPaidProgramming)
            return new XmltvProgramme()
            {
                // added +0000 for NPVR; otherwise it would assume local time instead of UTC
                Start = $"{scheduleEntry.StartTime:yyyyMMddHHmmss} +0000",
                Stop = $"{scheduleEntry.StartTime + TimeSpan.FromSeconds(scheduleEntry.Duration):yyyyMMddHHmmss} +0000",
                Channel = channelId,

                Titles = MxfStringToXmlTextArray(mxfProgram.Title),
                SubTitles = MxfStringToXmlTextArray(mxfProgram.EpisodeTitle),
                Descriptions = MxfStringToXmlTextArray((descriptionExtended + mxfProgram.Description).Trim()),
                Credits = BuildProgramCredits(mxfProgram),
                Date = BuildProgramDate(mxfProgram),
                Categories = BuildProgramCategories(mxfProgram),
                Language = MxfStringToXmlText(!string.IsNullOrEmpty(mxfProgram.Language) ? mxfProgram.Language[..2] : null),
                Icons = BuildProgramIcons(mxfProgram),
                Sport = GrabSportEvent(mxfProgram),
                Teams = BuildSportTeams(mxfProgram),
                EpisodeNums = BuildEpisodeNumbers(scheduleEntry),
                Video = BuildProgramVideo(scheduleEntry),
                Audio = BuildProgramAudio(scheduleEntry),
                PreviouslyShown = BuildProgramPreviouslyShown(scheduleEntry),
                Premiere = BuildProgramPremiere(scheduleEntry),
                Live = BuildLiveFlag(scheduleEntry),
                New = !scheduleEntry.IsRepeat ? string.Empty : null,
                Subtitles = BuildProgramSubtitles(scheduleEntry),
                Rating = BuildProgramRatings(scheduleEntry),
                StarRating = BuildProgramStarRatings(mxfProgram)
            };

        if (mxfProgram.IsMovie && mxfProgram.Year > 0) descriptionExtended = $"{mxfProgram.Year}";
        else if (!mxfProgram.IsMovie)
        {
            if (scheduleEntry.IsLive) descriptionExtended = "[LIVE]";
            else if (scheduleEntry.IsPremiere) descriptionExtended = "[PREMIERE]";
            else if (scheduleEntry.IsFinale) descriptionExtended = "[FINALE]";
            else if (!scheduleEntry.IsRepeat) descriptionExtended = "[NEW]";
            else if (scheduleEntry.IsRepeat && !mxfProgram.IsGeneric) descriptionExtended = "[REPEAT]";

            if (!settings.SDSettings.PrefixEpisodeTitle && !settings.SDSettings.PrefixEpisodeDescription && !settings.SDSettings.AppendEpisodeDesc)
            {
                if (mxfProgram.SeasonNumber > 0 && mxfProgram.EpisodeNumber > 0) descriptionExtended += $" S{mxfProgram.SeasonNumber}:E{mxfProgram.EpisodeNumber}";
                else if (mxfProgram.EpisodeNumber > 0) descriptionExtended += $" #{mxfProgram.EpisodeNumber}";
            }
        }

        //if (scheduleEntry.IsHdtv) descriptionExtended += " HD";
        //if (!string.IsNullOrEmpty(mxfProgram.Language)) descriptionExtended += $" {new CultureInfo(mxfProgram.Language).DisplayName}";
        //if (scheduleEntry.IsCC) descriptionExtended += " CC";
        //if (scheduleEntry.IsSigned) descriptionExtended += " Signed";
        //if (scheduleEntry.IsSap) descriptionExtended += " SAP";
        //if (scheduleEntry.IsSubtitled) descriptionExtended += " SUB";

        string[] tvRatings = [ "", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA",
                "", "Kinder bis 12 Jahren", "Freigabe ab 12 Jahren", "Freigabe ab 16 Jahren", "Keine Jugendfreigabe",
                "", "Déconseillé aux moins de 10 ans", "Déconseillé aux moins de 12 ans", "Déconseillé aux moins de 16 ans", "Déconseillé aux moins de 18 ans",
                "모든 연령 시청가", "7세 이상 시청가", "12세 이상 시청가", "15세 이상 시청가", "19세 이상 시청가",
                "SKY-UC", "SKY-U", "SKY-PG", "SKY-12", "SKY-15", "SKY-18", "SKY-R18" ];
        string[] mpaaRatings = ["", "G", "PG", "PG-13", "R", "NC-17", "X", "NR", "AO"];

        if (!string.IsNullOrEmpty(tvRatings[scheduleEntry.TvRating]))
        {
            descriptionExtended += $" {tvRatings[scheduleEntry.TvRating]}";
            if (mxfProgram.MpaaRating > 0) descriptionExtended += ",";
        }
        if (mxfProgram.MpaaRating > 0) descriptionExtended += $" {mpaaRatings[mxfProgram.MpaaRating]}";

        {
            var advisories = string.Empty;
            if (mxfProgram.HasAdult) advisories += "Adult Situations,";
            if (mxfProgram.HasGraphicLanguage) advisories += "Graphic Language,";
            else if (mxfProgram.HasLanguage) advisories += "Language,";
            if (mxfProgram.HasStrongSexualContent) advisories += "Strong Sexual Content,";
            if (mxfProgram.HasGraphicViolence) advisories += "Graphic Violence,";
            else if (mxfProgram.HasMildViolence) advisories += "Mild Violence,";
            else if (mxfProgram.HasViolence) advisories += "Violence,";
            if (mxfProgram.HasNudity) advisories += "Nudity,";
            else if (mxfProgram.HasBriefNudity) advisories += "Brief Nudity,";
            if (mxfProgram.HasRape) advisories += "Rape,";

            if (!string.IsNullOrEmpty(advisories)) descriptionExtended += $" ({advisories.Trim().TrimEnd(',').Replace(",", ", ")})";
        }

        if (mxfProgram.IsMovie && mxfProgram.HalfStars > 0)
        {
            descriptionExtended += $" {mxfProgram.HalfStars * 0.5:N1}/4.0";
        }
        else if (!mxfProgram.IsMovie)
        {
            if (!mxfProgram.IsGeneric && !string.IsNullOrEmpty(mxfProgram.OriginalAirdate)) descriptionExtended += $" Original air date: {DateTime.Parse(mxfProgram.OriginalAirdate):d}";
        }

        if (!string.IsNullOrEmpty(descriptionExtended)) descriptionExtended = descriptionExtended.Trim() + "\u000D\u000A";

        return new XmltvProgramme()
        {
            // added +0000 for NPVR; otherwise it would assume local time instead of UTC
            Start = $"{scheduleEntry.StartTime:yyyyMMddHHmmss} +0000",
            Stop = $"{scheduleEntry.StartTime + TimeSpan.FromSeconds(scheduleEntry.Duration):yyyyMMddHHmmss} +0000",
            Channel = channelId,

            Titles = MxfStringToXmlTextArray(mxfProgram.Title),
            SubTitles = MxfStringToXmlTextArray(mxfProgram.EpisodeTitle),
            Descriptions = MxfStringToXmlTextArray((descriptionExtended + mxfProgram.Description).Trim()),
            Credits = BuildProgramCredits(mxfProgram),
            Date = BuildProgramDate(mxfProgram),
            Categories = BuildProgramCategories(mxfProgram),
            Language = MxfStringToXmlText(!string.IsNullOrEmpty(mxfProgram.Language) ? mxfProgram.Language[..2] : null),
            Icons = BuildProgramIcons(mxfProgram),
            Sport = GrabSportEvent(mxfProgram),
            Teams = BuildSportTeams(mxfProgram),
            EpisodeNums = BuildEpisodeNumbers(scheduleEntry),
            Video = BuildProgramVideo(scheduleEntry),
            Audio = BuildProgramAudio(scheduleEntry),
            PreviouslyShown = BuildProgramPreviouslyShown(scheduleEntry),
            Premiere = BuildProgramPremiere(scheduleEntry),
            Live = BuildLiveFlag(scheduleEntry),
            New = (!scheduleEntry.IsRepeat) ? string.Empty : null,
            Subtitles = BuildProgramSubtitles(scheduleEntry),
            //Rating = BuildProgramRatings(mxfProgram, scheduleEntry),
            //StarRating = BuildProgramStarRatings(mxfProgram)
        };
    }

   
    // Titles, SubTitles, and Descriptions
    private static List<XmltvText>? MxfStringToXmlTextArray(string mxfString)
    {
        return string.IsNullOrEmpty(mxfString) ? null : [new() { Text = mxfString }];
    }

    // Credits
    private static XmltvCredit? BuildProgramCredits(MxfProgram mxfProgram)
    {
        if (mxfProgram.DirectorRole != null && mxfProgram.DirectorRole.Count > 0 || mxfProgram.ActorRole != null && mxfProgram.ActorRole.Count > 0 ||
            mxfProgram.WriterRole != null && mxfProgram.WriterRole.Count > 0 || mxfProgram.ProducerRole != null && mxfProgram.ProducerRole.Count > 0 ||
            mxfProgram.HostRole != null && mxfProgram.HostRole.Count > 0 || mxfProgram.GuestActorRole != null && mxfProgram.GuestActorRole.Count > 0)
        {
            return new XmltvCredit
            {
                Directors = MxfPersonRankToXmltvCrew(mxfProgram.DirectorRole),
                Actors = MxfPersonRankToXmltvActors(mxfProgram.ActorRole),
                Writers = MxfPersonRankToXmltvCrew(mxfProgram.WriterRole),
                Producers = MxfPersonRankToXmltvCrew(mxfProgram.ProducerRole),
                Presenters = MxfPersonRankToXmltvCrew(mxfProgram.HostRole),
                Guests = MxfPersonRankToXmltvCrew(mxfProgram.GuestActorRole)
            };
        }
        return null;
    }
    private static List<string>? MxfPersonRankToXmltvCrew(List<MxfPersonRank>? mxfPersons)
    {
        if ( mxfPersons is null) return null;
        return mxfPersons?.Select(person => person.Name).ToList();
    }
    private static List<XmltvActor>? MxfPersonRankToXmltvActors(List<MxfPersonRank>? mxfPersons)
    {
        if (mxfPersons is null) return null;
        return mxfPersons?.Select(person => new XmltvActor { Actor = person.Name, Role = person.Character }).ToList();
    }

    // Date
    private static string? BuildProgramDate(MxfProgram mxfProgram)
    {
        if (mxfProgram.IsMovie && mxfProgram.Year > 0) return mxfProgram.Year.ToString();
        return !string.IsNullOrEmpty(mxfProgram.OriginalAirdate) ? $"{DateTime.Parse(mxfProgram.OriginalAirdate):yyyyMMdd}" : null;
    }

    // Categories
    private  List<XmltvText>? BuildProgramCategories(MxfProgram mxfProgram)
    {
        if (string.IsNullOrEmpty(mxfProgram.Keywords)) return null;
        var categories = new HashSet<string>();
        foreach (var keywordId in mxfProgram.Keywords.Split(','))
        {
            foreach (var keyword in schedulesDirectData.Keywords.Where(keyword => !keyword.Word.ToLower().Contains("premiere")).Where(keyword => keyword.Id == keywordId))
            {
                if (keyword.Word.Equals("Uncategorized")) continue;
                categories.Add(keyword.Word.Equals("Movies") ? "Movie" : keyword.Word);
                break;
            }
        }

        if (categories.Contains("Kids") && categories.Contains("Children"))
        {
            categories.Remove("Kids");
        }

        return categories.Count <= 0 ? null : categories.Select(category => new XmltvText { Text = category }).ToList();
    }

    // Language
    private static XmltvText? MxfStringToXmlText(string mxfString)
    {
        return !string.IsNullOrEmpty(mxfString) ? new XmltvText { Text = mxfString } : null;
    }

    // Icons
    private  List<XmltvIcon>? BuildProgramIcons(MxfProgram mxfProgram)
    {
        var settings = memoryCache.GetSetting();
        if (settings.SDSettings.XmltvSingleImage)
        {
            var url = mxfProgram.mxfGuideImage?.ImageUrl ?? mxfProgram.mxfSeason?.mxfGuideImage?.ImageUrl ??
                mxfProgram.mxfSeriesInfo?.mxfGuideImage?.ImageUrl;
            if (url == null)
            {
                return null;
            }
            else
            {
                return (List<XmltvIcon>?)[new() { Src = GetIconUrl( url) }];
            }
        }

        var artwork = new List<ProgramArtwork>();

        // a movie or sport event will have a guide image from the program
        if (mxfProgram.extras.TryGetValue("artwork", out dynamic? value))
        {
            artwork = value;
        }

        // get the season class from the program if it is has a season
        if (artwork.Count == 0 && (mxfProgram.mxfSeason?.extras.ContainsKey("artwork") ?? false))
        {
            artwork = mxfProgram.mxfSeason.extras["artwork"];
        }

        // get the series info class from the program if it is a series
        if (artwork.Count == 0 && (mxfProgram.mxfSeriesInfo?.extras.ContainsKey("artwork") ?? false))
        {
            artwork = mxfProgram.mxfSeriesInfo.extras["artwork"];
        }

        return artwork.Count == 0 ? null : artwork.Select(image => new XmltvIcon { Src = GetIconUrl(image.Uri) , Height = image.Height, Width = image.Width }).ToList();
    }

    private static XmltvText? GrabSportEvent(MxfProgram program)
    {
        if (!program.IsSports || !program.extras.TryGetValue("genres", out dynamic? value)) return null;
        return (from category in ((string[])value) where !category.ToLower().StartsWith("sport") select new XmltvText { Text = category }).FirstOrDefault();
    }

    private static List<XmltvText>? BuildSportTeams(MxfProgram program)
    {
        if (!program.IsSports || !program.extras.TryGetValue("teams", out dynamic? value)) return null;
        return ((List<string>)value).Select(team => new XmltvText { Text = team }).ToList();
    }

    // EpisodeNums
    private static readonly Random RandomNumber = new();
    private  List<XmltvEpisodeNum> BuildEpisodeNumbers(MxfScheduleEntry mxfScheduleEntry)
    {
        var list = new List<XmltvEpisodeNum>();
        var mxfProgram = mxfScheduleEntry.mxfProgram;
        if (!mxfProgram.ProgramId.StartsWith("EPG123"))
        {
            list.Add(new XmltvEpisodeNum()
            {
                System = "dd_progid",
                Text = mxfProgram.Uid[9..].Replace("_", ".")
            });
        }

        if (mxfProgram.EpisodeNumber != 0 || mxfScheduleEntry.Part != 0)
        {
            var text =
                $"{(mxfProgram.SeasonNumber != 0 ? (mxfProgram.SeasonNumber - 1).ToString() : string.Empty)}.{(mxfProgram.EpisodeNumber != 0 ? (mxfProgram.EpisodeNumber - 1).ToString() : string.Empty)}.{(mxfScheduleEntry.Part != 0 ? $"{mxfScheduleEntry.Part - 1}/" : "0/")}{(mxfScheduleEntry.Parts != 0 ? $"{mxfScheduleEntry.Parts}" : "1")}";
            list.Add(new XmltvEpisodeNum() { System = "xmltv_ns", Text = text });
        }
        else if (mxfProgram.ProgramId.StartsWith("EPG123"))
        {
            // filler data - create oad of scheduled start time
            list.Add(new XmltvEpisodeNum() { System = "original-air-date", Text = $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}" });
        }
        else if (!mxfProgram.ProgramId.StartsWith("MV"))
        {
            // add this entry due to Plex identifying anything without an episode number as being a movie
            var oad = mxfProgram.OriginalAirdate;
            if (!mxfScheduleEntry.IsRepeat)
            {
                oad = $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd HH:mm}:{RandomNumber.Next(1, 60):00}";
            }
            else if (!string.IsNullOrEmpty(oad))
            {
                oad = $"{DateTime.Parse(oad):yyyy-MM-dd}";
            }
            else
            {
                oad = "1900-01-01";
            }
            list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = oad });
        }
        if (mxfProgram.Series == null) return list;

        var mxfSeriesInfo = schedulesDirectData.SeriesInfos[int.Parse(mxfProgram.Series[2..]) - 1];
        if (mxfSeriesInfo.extras.TryGetValue("tvdb", out dynamic? value))
        {
            list.Add(new XmltvEpisodeNum { System = "thetvdb.com", Text = $"series/{value}" });
        }
        return list;
    }

    // Video
    private static XmltvVideo? BuildProgramVideo(MxfScheduleEntry mxfScheduleEntry)
    {
        return mxfScheduleEntry.IsHdtv ? new XmltvVideo { Quality = "HDTV" } : null;
    }

    // Audio
    private static XmltvAudio? BuildProgramAudio(MxfScheduleEntry mxfScheduleEntry)
    {
        if (mxfScheduleEntry.AudioFormat <= 0) return null;
        var format = string.Empty;
        switch (mxfScheduleEntry.AudioFormat)
        {
            case 1: format = "mono"; break;
            case 2: format = "stereo"; break;
            case 3: format = "dolby"; break;
            case 4: format = "dolby digital"; break;
            case 5: format = "surround"; break;
        }
        return !string.IsNullOrEmpty(format) ? new XmltvAudio { Stereo = format } : null;
    }

    // Previously Shown
    private static XmltvPreviouslyShown? BuildProgramPreviouslyShown(MxfScheduleEntry mxfScheduleEntry)
    {
        if (mxfScheduleEntry.IsRepeat && !mxfScheduleEntry.mxfProgram.IsMovie)
        {
            return new XmltvPreviouslyShown { Text = string.Empty };
        }
        return null;
    }

    // Premiere
    private static XmltvText? BuildProgramPremiere(MxfScheduleEntry mxfScheduleEntry)
    {
        if (!mxfScheduleEntry.IsPremiere) return null;

        var mxfProgram = mxfScheduleEntry.mxfProgram;
        string text;
        if (mxfProgram.IsMovie) text = "Movie Premiere";
        else if (mxfProgram.IsSeriesPremiere) text = "Series Premiere";
        else if (mxfProgram.IsSeasonPremiere) text = "Season Premiere";
        else text = "Miniseries Premiere";

        return new XmltvText { Text = text };
    }

    private static string? BuildLiveFlag(MxfScheduleEntry mxfScheduleEntry)
    {
        return !mxfScheduleEntry.IsLive ? null : string.Empty;
    }

    // Subtitles
    private static List<XmltvSubtitles>? BuildProgramSubtitles(MxfScheduleEntry mxfScheduleEntry)
    {
        if (!mxfScheduleEntry.IsCc && !mxfScheduleEntry.IsSubtitled && !mxfScheduleEntry.IsSigned) return null;

        var list = new List<XmltvSubtitles>();
        if (mxfScheduleEntry.IsCc)
        {
            list.Add(new XmltvSubtitles() { Type = "teletext" });
        }
        if (mxfScheduleEntry.IsSubtitled)
        {
            list.Add(new XmltvSubtitles() { Type = "onscreen" });
        }
        if (mxfScheduleEntry.IsSigned)
        {
            list.Add(new XmltvSubtitles() { Type = "deaf-signed" });
        }
        return list;
    }

    // Rating
    private static List<XmltvRating> BuildProgramRatings(MxfScheduleEntry mxfScheduleEntry)
    {
        var ret = new List<XmltvRating>();
        var mxfProgram = mxfScheduleEntry.mxfProgram;
        AddProgramRatingAdvisory(mxfProgram.HasAdult, ret, "Adult Situations");
        AddProgramRatingAdvisory(mxfProgram.HasBriefNudity, ret, "Brief Nudity");
        AddProgramRatingAdvisory(mxfProgram.HasGraphicLanguage, ret, "Graphic Language");
        AddProgramRatingAdvisory(mxfProgram.HasGraphicViolence, ret, "Graphic Violence");
        AddProgramRatingAdvisory(mxfProgram.HasLanguage, ret, "Language");
        AddProgramRatingAdvisory(mxfProgram.HasMildViolence, ret, "Mild Violence");
        AddProgramRatingAdvisory(mxfProgram.HasNudity, ret, "Nudity");
        AddProgramRatingAdvisory(mxfProgram.HasRape, ret, "Rape");
        AddProgramRatingAdvisory(mxfProgram.HasStrongSexualContent, ret, "Strong Sexual Content");
        AddProgramRatingAdvisory(mxfProgram.HasViolence, ret, "Violence");

        AddProgramRating(mxfScheduleEntry, ret);
        return ret;
    }
    private static void AddProgramRating(MxfScheduleEntry mxfScheduleEntry, List<XmltvRating> list)
    {
        var hashSet = new HashSet<string>();
        if (mxfScheduleEntry.extras.TryGetValue("ratings", out dynamic? value))
        {
            foreach (KeyValuePair<string, string> rating in value)
            {
                if (hashSet.Contains(rating.Key)) continue;
                hashSet.Add(rating.Key);
                list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
            }
        }
        if (mxfScheduleEntry.mxfProgram.extras.TryGetValue("ratings", out dynamic? valueRatings))
        {
            foreach (KeyValuePair<string, string> rating in valueRatings)
            {
                if (hashSet.Contains(rating.Key)) continue;
                hashSet.Add(rating.Key);
                list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
            }
        }

        if (mxfScheduleEntry.TvRating != 0)
        {
            var rating = string.Empty;
            switch (mxfScheduleEntry.TvRating)
            {
                // v-chip is only for US, Canada, and Brazil
                case 1: rating = "TV-Y"; break;
                case 2: rating = "TV-Y7"; break;
                case 3: rating = "TV-G"; break;
                case 4: rating = "TV-PG"; break;
                case 5: rating = "TV-14"; break;
                case 6: rating = "TV-MA"; break;
            }
            if (!string.IsNullOrEmpty(rating))
            {
                list.Add(new XmltvRating { System = "VCHIP", Value = rating });
            }
        }
    }
    private static void AddProgramRatingAdvisory(bool mxfProgramAdvise, List<XmltvRating> list, string advisory)
    {
        if (mxfProgramAdvise)
        {
            list.Add(new XmltvRating { System = "advisory", Value = advisory });
        }
    }

    // StarRating
    private static List<XmltvRating>? BuildProgramStarRatings(MxfProgram mxfProgram)
    {
        if (mxfProgram.HalfStars == 0) return null;
        return
            [
                new() {
                    Value = $"{mxfProgram.HalfStars * 0.5:N1}/4"
                }
            ];
    }
    #endregion
}
