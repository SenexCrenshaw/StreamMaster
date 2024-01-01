using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Services;

using System.Globalization;
using System.Net;

namespace StreamMaster.SchedulesDirect;
public class XMLTVBuilder(IMemoryCache memoryCache, IEPGHelper ePGHelper, ISchedulesDirectDataService schedulesDirectDataService, ILogger<XMLTVBuilder> logger) : IXMLTVBuilder
{
    private string _baseUrl = "";
    //private ISchedulesDirectDataService schedulesDirectDataService;
    private readonly Dictionary<int, MxfSeriesInfo> seriesDict = [];
    private Dictionary<string, string> keywordDict = [];


    //[LogExecutionTimeAspect]
    public XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs)
    {

        _baseUrl = baseUrl;
        try
        {
            CreateDummyLineupChannels();

            XMLTV xmlTv = new()
            {
                Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                SourceInfoUrl = "http://schedulesdirect.org",
                SourceInfoName = "Schedules Direct",
                GeneratorInfoName = "StreamMaster",
                GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster/",
                Channels = [],
                Programs = []
            };

            Setting settings = memoryCache.GetSetting();
            List<MxfService> toProcess = [];

            int newServiceCount = 0;

            // Pre-process all keywords into a HashSet for faster lookup
            keywordDict = schedulesDirectDataService.AllKeywords
     .Where(k => !string.Equals(k.Word, "Uncategorized", StringComparison.OrdinalIgnoreCase) &&
                 !k.Word.Contains("premiere", StringComparison.OrdinalIgnoreCase))
     .GroupBy(k => k.Id)
     .ToDictionary(
         g => g.Key,
         g =>
         {
             string word = g.First().Word;
             return string.Equals(word, "Movies", StringComparison.OrdinalIgnoreCase) ? "Movie" : word;
         }
     );
            foreach (MxfSeriesInfo seriesInfo in schedulesDirectDataService.AllSeriesInfos)
            {
                if (seriesDict.ContainsKey(seriesInfo.Index))
                {
                    MxfSeriesInfo a = seriesDict[seriesInfo.Index];
                    // Handle the duplicate key scenario, e.g., log it or throw an exception
                    // LogWarning($"Duplicate series index found: {seriesInfo.Index}");
                    continue;
                }
                seriesDict.Add(seriesInfo.Index, seriesInfo);
            }

            List<MxfService> services = schedulesDirectDataService.AllServices;

            HashSet<int> chNos = [];
            HashSet<int> existingChNos = videoStreamConfigs.Select(a => a.User_Tvg_chno).Distinct().ToHashSet();


            foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs.OrderBy(a => a.User_Tvg_chno))
            {
                string prefix = videoStreamConfig.IsDummy ? "DUMMY" : "SM";
                int epgNumber;
                string stationId;

                if (videoStreamConfig.Id == "282476628d303b54eaec5b63457d0447")
                {
                    var aa = 1;
                }
                (epgNumber, stationId) = ePGHelper.ExtractEPGNumberAndStationId(videoStreamConfig.User_Tvg_ID);
                //}

                MxfService? origService = services.FirstOrDefault(a => a.StationId == stationId && a.EPGNumber == epgNumber);

                if (origService == null)
                {
                    continue;
                }

                MxfService newService = new(newServiceCount++, videoStreamConfig.User_Tvg_ID);// schedulesDirectDataService.FindOrCreateService(stationId);

                if (origService.MxfScheduleEntries is not null)
                {
                    newService.MxfScheduleEntries = origService.MxfScheduleEntries;
                }

                int chNo = videoStreamConfig.User_Tvg_chno;
                if (chNos.Contains(chNo))
                {
                    foreach (var num in existingChNos.Concat(chNos))
                    {
                        if (num != chNo)
                        {
                            break;
                        }
                        chNo++;
                    }
                }
                chNos.Add(chNo);

                newService.EPGNumber = epgNumber;
                newService.ChNo = chNo;
                newService.Name = videoStreamConfig.User_Tvg_name;
                newService.Affiliate = origService.Affiliate;
                newService.CallSign = origService.CallSign;
                newService.LogoImage = videoStreamConfig.User_Tvg_Logo;

                newService.extras = origService.extras;
                newService.extras.AddOrUpdate("videoStreamConfig", videoStreamConfig);

                if (!settings.VideoStreamAlwaysUseEPGLogo && !string.IsNullOrEmpty(videoStreamConfig.User_Tvg_Logo))
                {
                    if (newService.extras.TryGetValue("logo", out dynamic? value))
                    {
                        value.Url = videoStreamConfig.User_Tvg_Logo;
                    }
                    else
                    {

                        newService.extras.Add("logo", new StationImage
                        {
                            Url = videoStreamConfig.User_Tvg_Logo

                        });
                    }
                }

                toProcess.Add(newService);

            }

            List<MxfProgram> programs = schedulesDirectDataService.AllPrograms;

            try
            {
                Parallel.ForEach(toProcess, service =>
                {

                    xmlTv.Channels.Add(BuildXmltvChannel(service, videoStreamConfigs));

                    if (service.MxfScheduleEntries.ScheduleEntry.Count == 0 && settings.SDSettings.XmltvAddFillerData)
                    {
                        // add a program specific for this service
                        MxfProgram program = new(programs.Count + 1, $"SM-{service.StationId}")
                        {
                            Title = service.Name,
                            Description = settings.SDSettings.XmltvFillerProgramDescription,
                            IsGeneric = true
                        };

                        // populate the schedule entries
                        DateTime startTime = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                        DateTime stopTime = startTime + TimeSpan.FromDays(settings.SDSettings.SDEPGDays);
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

                    List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;

                    string channelId = service.ChNo.ToString();

                    Parallel.ForEach(service.MxfScheduleEntries.ScheduleEntry, scheduleEntry =>
                {
                    XmltvProgramme program = BuildXmltvProgram(scheduleEntry, channelId);
                    xmlTv.Programs.Add(program);
                });

                });

                List<XmltvProgramme> a = xmlTv.Programs.Where(a => a == null || a.Channel == null || a.StartDateTime == null).ToList();

                xmlTv.Channels = xmlTv.Channels
          .Select(c => new { Channel = c, IsNumeric = int.TryParse(c.Id, out var num), NumericId = num })
          .OrderBy(c => c.IsNumeric)
          .ThenBy(c => c.NumericId)
          .Select(c => c.Channel)
          .ToList();

                xmlTv.Programs = xmlTv.Programs
        .Select(c => new { Program = c, IsNumeric = int.TryParse(c.Channel, out var num), NumericId = num })
        .OrderBy(c => c.IsNumeric)
        .ThenBy(c => c.NumericId)
        .ThenBy(c => c.Program.StartDateTime)
        .Select(c => c.Program)
        .ToList();

            }
            catch (Exception ex)
            {
                return xmlTv;
            }

            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Failed to create the XMLTV file. Exception:{FileUtil.ReportExceptionMessages(ex)}");
        }
        return null;
    }

    private void CreateDummyLineupChannels()
    {
        //using IServiceScope scope = serviceProvider.CreateScope();
        //IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        //List<VideoStream> dummies = [.. repository.VideoStream.FindByCondition(x => x.User_Tvg_ID == "DUMMY")];

        //foreach (VideoStream dummy in dummies)
        //{
        //    string dummyName = "DUMMY-" + dummy.Id;

        //    MxfService mxfService = schedulesDirectData.FindOrCreateService(dummyName);
        //    mxfService.CallSign = dummy.User_Tvg_name;
        //    mxfService.Name = dummy.User_Tvg_name;

        //    MxfLineup mxfLineup = schedulesDirectData.FindOrCreateLineup($"ZZZ-{dummyName}-StreamMaster", $"ZZZSM {dummyName} Lineup");
        //    mxfLineup.channels.Add(new MxfChannel(mxfLineup, mxfService));
        //}
    }

    private string GetIconUrl(int EPGNumber, string iconOriginalSource, SMFileTypes? sMFileTypes = null)
    {
        if (ePGHelper.IsDummy(EPGNumber))
        {
            return iconOriginalSource;
        }

        if (ePGHelper.IsSchedulesDirect(EPGNumber))
        {
            if (iconOriginalSource.StartsWith("http"))
            {
                return iconOriginalSource;
            }
            else
            {
                return GetApiUrl(sMFileTypes ?? SMFileTypes.SDImage, iconOriginalSource);
            }
        }


        Setting settings = memoryCache.GetSetting();

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

        if (settings.CacheIcons)
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
    private XmltvChannel BuildXmltvChannel(MxfService mxfService, List<VideoStreamConfig> videoStreamConfigs)
    {
        Setting settings = memoryCache.GetSetting();

        // initialize the return channel
        XmltvChannel ret = new()
        {
            Id = mxfService.ChNo.ToString(),
            DisplayNames = []
        };

        ret.DisplayNames.Add(new XmltvText { Text = mxfService.CallSign });
        if (!mxfService.Name.Equals(mxfService.CallSign))
        {
            ret.DisplayNames.Add(new XmltvText { Text = mxfService.Name });
        }

        // add channel number if requested
        if (settings.SDSettings.XmltvIncludeChannelNumbers)
        {
            HashSet<string> numbers = [];

            foreach (MxfLineup mxfLineup in schedulesDirectDataService.AllLineups)
            {
                foreach (MxfChannel mxfChannel in mxfLineup.channels)
                {
                    if (mxfChannel.Service != mxfService.Id || mxfChannel.Number <= 0)
                    {
                        continue;
                    }

                    string num = $"{mxfChannel.Number}" + (mxfChannel.SubNumber > 0 ? $".{mxfChannel.SubNumber}" : "");
                    if (!numbers.Add(num))
                    {
                        continue;
                    }

                    ret.DisplayNames.Add(new XmltvText { Text = num + " " + mxfService.CallSign });
                    ret.DisplayNames.Add(new XmltvText { Text = num });
                }
            }
        }

        // add affiliate if present
        string? affiliate = mxfService.mxfAffiliate?.Name;
        if (!string.IsNullOrEmpty(affiliate) && !mxfService.Name.Equals(affiliate))
        {
            ret.DisplayNames.Add(new XmltvText { Text = affiliate });
        }

        if (mxfService.EPGNumber < 0)
        {
            var a = 1;
        }

        // add logo if available
        if (mxfService.extras.TryGetValue("logo", out dynamic? logos))
        {

            ret.Icons =
                [
                    new() {
                        Src = GetIconUrl(mxfService.EPGNumber, mxfService.extras["logo"].Url),
                        Height = mxfService.extras["logo"].Height,
                        Width = mxfService.extras["logo"].Width
                    }
                ];

        }
        return ret;
    }
    #endregion

    #region ========== XMLTV Programmes and Functions ==========

    private XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId)
    {
        Setting settings = memoryCache.GetSetting();
        MxfProgram mxfProgram = scheduleEntry.mxfProgram;
        string descriptionExtended = string.Empty;
        if (!settings.SDSettings.XmltvExtendedInfoInTitleDescriptions || mxfProgram.IsPaidProgramming)
        {
            //List<string> svcs = schedulesDirectData.Services.Select(a => a.StationId).ToList();
            //MxfService? svc = schedulesDirectData.GetService(channelId);
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
                SubTitles2 = BuildProgramSubtitles(scheduleEntry),
                Rating = BuildProgramRatings(scheduleEntry),
                StarRating = BuildProgramStarRatings(mxfProgram)
            };
        }

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

            if (!settings.SDSettings.PrefixEpisodeTitle && !settings.SDSettings.PrefixEpisodeDescription && !settings.SDSettings.AppendEpisodeDesc)
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
            if (mxfProgram.MpaaRating > 0)
            {
                descriptionExtended += ",";
            }
        }
        if (mxfProgram.MpaaRating > 0)
        {
            descriptionExtended += $" {mpaaRatings[mxfProgram.MpaaRating]}";
        }

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

            if (!string.IsNullOrEmpty(advisories))
            {
                descriptionExtended += $" ({advisories.Trim().TrimEnd(',').Replace(",", ", ")})";
            }
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
            descriptionExtended = descriptionExtended.Trim() + "\u000D\u000A";
        }

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
            SubTitles2 = BuildProgramSubtitles(scheduleEntry),
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

    private XmltvCredit? BuildProgramCredits(MxfProgram mxfProgram)
    {
        // Check if there are any roles with associated persons
        bool hasCredits = mxfProgram.DirectorRole?.Any() == true ||
                          mxfProgram.ActorRole?.Any() == true ||
                          mxfProgram.WriterRole?.Any() == true ||
                          mxfProgram.ProducerRole?.Any() == true ||
                          mxfProgram.HostRole?.Any() == true ||
                          mxfProgram.GuestActorRole?.Any() == true;

        if (!hasCredits)
        {
            return null;
        }

        // Construct and return the XmltvCredit object
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

    private static List<string>? MxfPersonRankToXmltvCrew(List<MxfPersonRank>? mxfPersons)
    {
        return mxfPersons is null ? (List<string>?)null : (mxfPersons?.Select(person => person.Name).ToList());
    }
    private static List<XmltvActor>? MxfPersonRankToXmltvActors(List<MxfPersonRank>? mxfPersons)
    {
        return mxfPersons is null
            ? (List<XmltvActor>?)null
            : (mxfPersons?.Select(person => new XmltvActor { Actor = person.Name, Role = person.Character }).ToList());
    }

    // Date
    private static string? BuildProgramDate(MxfProgram mxfProgram)
    {
        return mxfProgram.IsMovie && mxfProgram.Year > 0
            ? mxfProgram.Year.ToString()
            : !string.IsNullOrEmpty(mxfProgram.OriginalAirdate) ? $"{DateTime.Parse(mxfProgram.OriginalAirdate):yyyyMMdd}" : null;
    }

    // Categories

    private List<XmltvText>? BuildProgramCategories(MxfProgram mxfProgram)
    {
        if (string.IsNullOrEmpty(mxfProgram.Keywords))
        {
            return null;
        }

        HashSet<string> categories = [];

        foreach (string keywordId in mxfProgram.Keywords.Split(','))
        {
            if (keywordDict.TryGetValue(keywordId, out string? word))
            {
                categories.Add(word);
            }
        }

        // Remove "Kids" if "Children" is also present
        if (categories.Contains("Movies"))
        {
            categories.Remove("Movies");
            if (!categories.Contains("Movie"))
            {
                categories.Add("Movie");
            }
        }

        // Remove "Kids" if "Children" is also present
        if (categories.Contains("Children"))
        {
            categories.Remove("Kids");
        }

        return categories.Count == 0 ? null : categories.Select(category => new XmltvText { Text = category }).ToList();
    }


    // Language    
    private static XmltvText? MxfStringToXmlText(string? mxfString)
    {
        return !string.IsNullOrEmpty(mxfString) ? new XmltvText { Text = mxfString } : null;
    }

    // Icons    
    private List<XmltvIcon>? BuildProgramIcons(MxfProgram mxfProgram)
    {
        Setting settings = memoryCache.GetSetting();


        if (settings.SDSettings.XmltvSingleImage || !mxfProgram.extras.ContainsKey("artwork"))
        {
            // Use the first available image URL
            string? url = mxfProgram.mxfGuideImage?.ImageUrl ??
                          mxfProgram.mxfSeason?.mxfGuideImage?.ImageUrl ??
                          mxfProgram.mxfSeriesInfo?.mxfGuideImage?.ImageUrl;

            return url != null ? [new XmltvIcon { Src = GetIconUrl(mxfProgram.EPGNumber, url) }] : null;
        }

        // Retrieve artwork from the program, season, or series info
        List<ProgramArtwork>? artwork = mxfProgram.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                       mxfProgram.mxfSeason?.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                       mxfProgram.mxfSeriesInfo?.extras.GetValueOrDefault("artwork") as List<ProgramArtwork>;

        // Convert artwork to XmltvIcon list
        return artwork?.Select(image => new XmltvIcon
        {
            Src = GetIconUrl(mxfProgram.EPGNumber, image.Uri),
            Height = image.Height,
            Width = image.Width
        }).ToList();
    }

    private static XmltvText? GrabSportEvent(MxfProgram program)
    {
        return !program.IsSports || !program.extras.TryGetValue("genres", out dynamic? value)
            ? null
            : (from category in (string[])value where !category.ToLower().StartsWith("sport") select new XmltvText { Text = category }).FirstOrDefault();
    }


    private static List<XmltvText>? BuildSportTeams(MxfProgram program)
    {
        return !program.IsSports || !program.extras.TryGetValue("teams", out dynamic? value)
            ? (List<XmltvText>?)null
            : ((List<string>)value).Select(team => new XmltvText { Text = team }).ToList();
    }

    // EpisodeNums

    private static readonly Random RandomNumber = new();

    private List<XmltvEpisodeNum> BuildEpisodeNumbers(MxfScheduleEntry mxfScheduleEntry)
    {
        List<XmltvEpisodeNum> list = [];
        MxfProgram mxfProgram = mxfScheduleEntry.mxfProgram;

        if (!mxfProgram.ProgramId.StartsWith("StreamMaster"))
        {
            list.Add(new XmltvEpisodeNum
            {
                System = "dd_progid",
                Text = mxfProgram.Uid[9..].Replace("_", ".")
            });
        }

        if (mxfProgram.EpisodeNumber != 0 || mxfScheduleEntry.Part != 0)
        {
            string seasonPart = mxfProgram.SeasonNumber > 0 ? (mxfProgram.SeasonNumber - 1).ToString() : "";
            string episodePart = mxfProgram.EpisodeNumber > 0 ? (mxfProgram.EpisodeNumber - 1).ToString() : "";
            string part = mxfScheduleEntry.Part > 0 ? $"{mxfScheduleEntry.Part - 1}/" : "0/";
            string parts = mxfScheduleEntry.Parts > 0 ? mxfScheduleEntry.Parts.ToString() : "1";
            string text = $"{seasonPart}.{episodePart}.{part}{parts}";

            list.Add(new XmltvEpisodeNum { System = "xmltv_ns", Text = text });
        }
        else if (mxfProgram.ProgramId.StartsWith("StreamMaster"))
        {
            list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}" });
        }
        else if (!mxfProgram.ProgramId.StartsWith("MV"))
        {
            string? oad = mxfProgram.OriginalAirdate;
            oad = !mxfScheduleEntry.IsRepeat
                ? $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd HH:mm}:{RandomNumber.Next(1, 60):00}"
                : !string.IsNullOrEmpty(oad) ? $"{DateTime.Parse(oad):yyyy-MM-dd}" : "1900-01-01";
            list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = oad });
        }

        if (mxfProgram.Series != null)
        {
            if (seriesDict.TryGetValue(int.Parse(mxfProgram.Series[2..]) - 1, out MxfSeriesInfo? mxfSeriesInfo) &&
                mxfSeriesInfo.extras.TryGetValue("tvdb", out dynamic value))
            {
                list.Add(new XmltvEpisodeNum { System = "thetvdb.com", Text = $"series/{value}" });
            }
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
        if (mxfScheduleEntry.AudioFormat <= 0)
        {
            return null;
        }

        string format = string.Empty;
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
        return mxfScheduleEntry.IsRepeat && !mxfScheduleEntry.mxfProgram.IsMovie ? new XmltvPreviouslyShown { Text = string.Empty } : null;
    }

    // Premiere    
    private static XmltvText? BuildProgramPremiere(MxfScheduleEntry mxfScheduleEntry)
    {
        if (!mxfScheduleEntry.IsPremiere)
        {
            return null;
        }

        MxfProgram mxfProgram = mxfScheduleEntry.mxfProgram;
        string text = mxfProgram.IsMovie
            ? "Movie Premiere"
            : mxfProgram.IsSeriesPremiere ? "Series Premiere" : mxfProgram.IsSeasonPremiere ? "Season Premiere" : "Miniseries Premiere";
        return new XmltvText { Text = text };
    }


    private static string? BuildLiveFlag(MxfScheduleEntry mxfScheduleEntry)
    {
        return !mxfScheduleEntry.IsLive ? null : string.Empty;
    }

    // Subtitles    
    private static List<XmltvSubtitles>? BuildProgramSubtitles(MxfScheduleEntry mxfScheduleEntry)
    {
        if (!mxfScheduleEntry.IsCc && !mxfScheduleEntry.IsSubtitled && !mxfScheduleEntry.IsSigned)
        {
            return null;
        }

        List<XmltvSubtitles> list = [];
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
        List<XmltvRating> ret = [];
        MxfProgram mxfProgram = mxfScheduleEntry.mxfProgram;
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
        HashSet<string> hashSet = [];
        if (mxfScheduleEntry.extras.TryGetValue("ratings", out dynamic? value))
        {
            foreach (KeyValuePair<string, string> rating in value)
            {
                if (hashSet.Contains(rating.Key))
                {
                    continue;
                }

                hashSet.Add(rating.Key);
                list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
            }
        }
        if (mxfScheduleEntry.mxfProgram.extras.TryGetValue("ratings", out dynamic? valueRatings))
        {
            foreach (KeyValuePair<string, string> rating in valueRatings)
            {
                if (hashSet.Contains(rating.Key))
                {
                    continue;
                }

                hashSet.Add(rating.Key);
                list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
            }
        }

        if (mxfScheduleEntry.TvRating != 0)
        {
            string rating = string.Empty;
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
        return mxfProgram.HalfStars == 0
            ? null
            : ([
                new() {
                    Value = $"{mxfProgram.HalfStars * 0.5:N1}/4"
                }
            ]);
    }
    #endregion
}
