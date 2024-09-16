using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Comparer;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;

using System.Collections.Concurrent;
using System.Globalization;

namespace StreamMaster.SchedulesDirect.Converters
{
    public class XMLTVBuilder : IXMLTVBuilder
    {
        private readonly IOptionsMonitor<SDSettings> _sdSettingsMonitor;
        private readonly IOptionsMonitor<OutputProfileDict> _outputProfileDictMonitor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICustomPlayListBuilder _customPlayListBuilder;
        private readonly IIconHelper _iconHelper;
        private readonly ISchedulesDirectDataService _schedulesDirectDataService;
        private readonly ILogger<XMLTVBuilder> _logger;

        private string _baseUrl = string.Empty;

        // Instance dictionaries to avoid static state
        private readonly ConcurrentDictionary<int, SeriesInfo> _seriesDict = new();
        private ConcurrentDictionary<string, string> _keywordDict = new();

        // Program cache
        private readonly ConcurrentDictionary<string, MxfProgram> _programsByTitle = new();

        // Ratings arrays
        private static readonly string[] TvRatings = new string[]
        {
            "", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA",
            "", "Kinder bis 12 Jahren", "Freigabe ab 12 Jahren", "Freigabe ab 16 Jahren", "Keine Jugendfreigabe",
            "", "Déconseillé aux moins de 10 ans", "Déconseillé aux moins de 12 ans", "Déconseillé aux moins de 16 ans", "Déconseillé aux moins de 18 ans",
            "모든 연령 시청가", "7세 이상 시청가", "12세 이상 시청가", "15세 이상 시청가", "19세 이상 시청가",
            "SKY-UC", "SKY-U", "SKY-PG", "SKY-12", "SKY-15", "SKY-18", "SKY-R18"
        };

        private static readonly string[] MpaaRatings = new string[]
        {
            "", "G", "PG", "PG-13", "R", "NC-17", "X", "NR", "AO"
        };

        public XMLTVBuilder(
            IOptionsMonitor<SDSettings> sdSettingsMonitor,
            IOptionsMonitor<OutputProfileDict> outputProfileDictMonitor,
            IServiceProvider serviceProvider,
            ICustomPlayListBuilder customPlayListBuilder,
            IIconHelper iconHelper,
            ISchedulesDirectDataService schedulesDirectDataService,
            ILogger<XMLTVBuilder> logger)
        {
            _sdSettingsMonitor = sdSettingsMonitor;
            _outputProfileDictMonitor = outputProfileDictMonitor;
            _serviceProvider = serviceProvider;
            _customPlayListBuilder = customPlayListBuilder;
            _iconHelper = iconHelper;
            _schedulesDirectDataService = schedulesDirectDataService;
            _logger = logger;
        }

        public XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs, OutputProfileDto outputProfile)
        {
            _baseUrl = baseUrl;

            try
            {
                InitializeDataDictionaries();

                List<MxfService> servicesToProcess = GetServicesToProcess(videoStreamConfigs);

                XMLTV xmlTv = InitializeXmlTv();

                ProcessPrograms(servicesToProcess, xmlTv, outputProfile, videoStreamConfigs);

                SortXmlTvEntries(xmlTv);

                return xmlTv;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create the XMLTV file. Exception: {ex.Message}");
                return null;
            }
        }

        public XMLTV? CreateSDXmlTv(string baseUrl)
        {
            _baseUrl = baseUrl;

            try
            {
                InitializeDataDictionaries();

                List<MxfService> services = _schedulesDirectDataService.GetAllSDServices;
                XMLTV xmlTv = InitializeXmlTv();
                OutputProfileDto outputProfile = _outputProfileDictMonitor.CurrentValue.GetDefaultProfileDto();

                ProcessPrograms(services, xmlTv, outputProfile);

                SortXmlTvEntries(xmlTv);

                return xmlTv;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create the XMLTV file. Exception: {ex.Message}");
                return null;
            }
        }

        private void InitializeDataDictionaries()
        {
            // Initialize _seriesDict and _keywordDict
            _seriesDict.Clear();
            _keywordDict.Clear();

            foreach (SeriesInfo seriesInfo in _schedulesDirectDataService.AllSeriesInfos)
            {
                _seriesDict.TryAdd(seriesInfo.Index, seriesInfo);
            }

            _keywordDict = new ConcurrentDictionary<string, string>(
                _schedulesDirectDataService.AllKeywords
                    .Where(k => !string.Equals(k.Word, "Uncategorized", StringComparison.OrdinalIgnoreCase)
                                && !k.Word.Contains("premiere", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(k => k.Id)
                    .ToDictionary(
                        g => g.Key,
                        g =>
                        {
                            string word = g.First().Word;
                            return string.Equals(word, "Movies", StringComparison.OrdinalIgnoreCase) ? "Movie" : word;
                        }
                    )
            );
        }

        private List<MxfService> GetServicesToProcess(List<VideoStreamConfig> videoStreamConfigs)
        {
            List<MxfService> servicesToProcess = [];
            List<MxfService> allServices = _schedulesDirectDataService.AllServices.OrderBy(a => a.EPGNumber).ToList();
            int newServiceCount = 0;

            foreach (VideoStreamConfig? videoStreamConfig in videoStreamConfigs.OrderBy(a => a.ChannelNumber))
            {
                MxfService? origService = FindOriginalService(videoStreamConfig, allServices);

                if (origService == null)
                {
                    // Handle cases where the original service is not found
                    origService = HandleMissingOriginalService(videoStreamConfig);
                    if (origService == null)
                    {
                        continue;
                    }
                }

                MxfService newService = CreateNewService(videoStreamConfig, origService, newServiceCount++);
                servicesToProcess.Add(newService);
            }

            return servicesToProcess;
        }

        private MxfService? FindOriginalService(VideoStreamConfig videoStreamConfig, List<MxfService> allServices)
        {
            // Find the original service based on EPGId
            MxfService? origService = allServices.FirstOrDefault(s => s.StationId == videoStreamConfig.EPGId);
            return origService;
        }

        private MxfService? HandleMissingOriginalService(VideoStreamConfig videoStreamConfig)
        {
            // Handle cases where the original service is not found
            if (videoStreamConfig.EPGId.StartsWith(EPGHelper.CustomPlayListId.ToString()))
            {
                string callsign = videoStreamConfig.EPGId;
                if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
                {
                    (_, callsign) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                }
                MxfService origService = _schedulesDirectDataService.CustomStreamData().FindOrCreateService(videoStreamConfig.EPGId);
                origService.EPGNumber = EPGHelper.CustomPlayListId;
                origService.CallSign = callsign;
                return origService;
            }
            else if (!videoStreamConfig.EPGId.StartsWith(EPGHelper.DummyId.ToString()) && !videoStreamConfig.EPGId.StartsWith(EPGHelper.SchedulesDirectId.ToString()))
            {
                MxfService origService = _schedulesDirectDataService.DummyData().FindOrCreateService(videoStreamConfig.EPGId);
                origService.EPGNumber = EPGHelper.DummyId;
                origService.CallSign = videoStreamConfig.EPGId;
                return origService;
            }
            else
            {
                return null;
            }
        }

        private MxfService CreateNewService(VideoStreamConfig videoStreamConfig, MxfService origService, int serviceId)
        {
            // Create a new MxfService based on the original service and video stream config
            MxfService newService = new(serviceId, videoStreamConfig.EPGId)
            {
                EPGNumber = origService.EPGNumber,
                ChNo = videoStreamConfig.ChannelNumber,
                Name = videoStreamConfig.Name,
                Affiliate = origService.Affiliate,
                CallSign = origService.CallSign,
                LogoImage = videoStreamConfig.Logo,
                extras = new ConcurrentDictionary<string, dynamic>(origService.extras),
                MxfScheduleEntries = origService.MxfScheduleEntries
            };

            newService.extras.AddOrUpdate("videoStreamConfig", videoStreamConfig, (key, oldValue) => videoStreamConfig);

            if (!string.IsNullOrEmpty(videoStreamConfig.Logo))
            {
                newService.extras.AddOrUpdate("logo", new StationImage
                {
                    Url = videoStreamConfig.Logo
                }, (key, oldValue) => new StationImage { Url = videoStreamConfig.Logo });
            }

            return newService;
        }

        private XMLTV InitializeXmlTv()
        {
            return new XMLTV
            {
                Date = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                SourceInfoName = "Stream Master",
                GeneratorInfoName = "Stream Master",
                GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                Channels = [],
                Programs = []
            };
        }

        private void ProcessPrograms(List<MxfService> services, XMLTV xmlTv, OutputProfileDto outputProfile, List<VideoStreamConfig>? videoStreamConfigs = null)
        {
            _programsByTitle.Clear();
            videoStreamConfigs ??= [];

            SDSettings sdSettings = _sdSettingsMonitor.CurrentValue;
            List<EPGFile> epgFiles = GetEPGFiles(videoStreamConfigs);

            try
            {
                Parallel.ForEach(services, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, service =>
                {
                    VideoStreamConfig? videoStreamConfig = videoStreamConfigs.Find(a => service.StationId == a.EPGId);
                    XmltvChannel channel = BuildXmltvChannel(service, videoStreamConfig, outputProfile);
                    lock (xmlTv.Channels)
                    {
                        xmlTv.Channels.Add(channel);
                    }

                    AdjustServiceSchedules(service, videoStreamConfig, sdSettings, epgFiles);

                    List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;
                    int timeShift = GetTimeShift(videoStreamConfig, epgFiles);

                    List<XmltvProgramme> xmltvProgrammes = scheduleEntries.AsParallel().Select(scheduleEntry =>
                        BuildXmltvProgram(scheduleEntry, channel.Id, timeShift)).ToList();

                    lock (xmlTv.Programs)
                    {
                        xmlTv.Programs.AddRange(xmltvProgrammes);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing programs.");
            }
        }

        private List<EPGFile> GetEPGFiles(List<VideoStreamConfig> videoStreamConfigs)
        {
            List<EPGFile> epgFiles = [];

            if (videoStreamConfigs.Any())
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IRepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<IRepositoryContext>();
                epgFiles = repositoryContext.EPGFiles.ToList();
            }

            return epgFiles;
        }

        private void AdjustServiceSchedules(MxfService service, VideoStreamConfig? videoStreamConfig, SDSettings sdSettings, List<EPGFile> epgFiles)
        {
            // Add filler data if necessary
            if (service.MxfScheduleEntries.ScheduleEntry.Count == 0 && sdSettings.XmltvAddFillerData)
            {
                if (service.EPGNumber == EPGHelper.CustomPlayListId)
                {
                    // Custom playlist logic
                    List<(Movie Movie, DateTime StartTime, DateTime EndTime)> moviesForPeriod = _customPlayListBuilder.GetMoviesForPeriod(service.CallSign, DateTime.UtcNow, sdSettings.SDEPGDays);

                    foreach ((Movie movie, DateTime startTime, DateTime endTime) in moviesForPeriod)
                    {
                        int duration = (int)(endTime - startTime).TotalMinutes;
                        if (duration <= 0)
                        {
                            continue;
                        }

                        MxfProgram program = GetOrCreateProgram(movie.Id, movie.Title, service.EPGNumber);
                        service.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
                        {
                            Duration = duration,
                            mxfProgram = program,
                            StartTime = startTime,
                            IsRepeat = true
                        });
                    }
                }
                else
                {
                    // Filler data for other services
                    MxfProgram program = new(0, $"SM-{service.StationId}")
                    {
                        Title = service.Name,
                        Description = service.Name,
                        IsGeneric = true
                    };

                    DateTime startTime = DateTime.UtcNow.Date;
                    DateTime stopTime = startTime.AddDays(sdSettings.SDEPGDays);

                    while (startTime < stopTime)
                    {
                        service.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
                        {
                            Duration = sdSettings.XmltvFillerProgramLength * 60 * 60, // Duration in seconds
                            mxfProgram = program,
                            StartTime = startTime,
                            IsRepeat = true
                        });
                        startTime = startTime.AddHours(sdSettings.XmltvFillerProgramLength);
                    }
                }
            }
        }

        private int GetTimeShift(VideoStreamConfig? videoStreamConfig, List<EPGFile> epgFiles)
        {
            int timeShift = 0;

            if (videoStreamConfig != null)
            {
                if (videoStreamConfig.TimeShift > 0)
                {
                    timeShift = videoStreamConfig.TimeShift;
                }
                else if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
                {
                    (int epgNumber, _) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                    if (epgNumber > 0)
                    {
                        EPGFile? epg = epgFiles.Find(a => a.EPGNumber == epgNumber);
                        if (epg != null)
                        {
                            timeShift = epg.TimeShift;
                        }
                    }
                }
            }

            return timeShift;
        }

        private XmltvChannel BuildXmltvChannel(MxfService service, VideoStreamConfig? videoStreamConfig, OutputProfileDto outputProfile)
        {
            string id = GetChannelId(service, videoStreamConfig, outputProfile);

            XmltvChannel channel = new()
            {
                Id = id,
                DisplayNames = []
            };

            string displayName = service.Name ?? service.CallSign;
            channel.DisplayNames.Add(new XmltvText { Text = displayName });

            // Add additional display names if necessary
            if (!string.IsNullOrEmpty(service.CallSign) && !service.CallSign.Equals(displayName))
            {
                channel.DisplayNames.Add(new XmltvText { Text = service.CallSign });
            }

            // Add logo if available
            if (service.extras.TryGetValue("logo", out dynamic logoObj))
            {
                if (logoObj is StationImage stationImage)
                {
                    channel.Icons =
                    [
                        new XmltvIcon
                        {
                            Src = _iconHelper.GetIconUrl(service.EPGNumber, stationImage.Url, _baseUrl),
                            Height = stationImage.Height,
                            Width = stationImage.Width
                        }
                    ];
                }
            }

            return channel;
        }

        private string GetChannelId(MxfService service, VideoStreamConfig? videoStreamConfig, OutputProfileDto outputProfile)
        {
            string id = service.ChNo.ToString();

            if (outputProfile.Id != nameof(ValidM3USetting.NotMapped))
            {
                switch (outputProfile.Id)
                {
                    case nameof(ValidM3USetting.Group):
                        if (videoStreamConfig != null && !string.IsNullOrEmpty(videoStreamConfig.Group))
                        {
                            id = videoStreamConfig.Group;
                        }
                        break;
                    case nameof(ValidM3USetting.ChannelNumber):
                        id = service.ChNo.ToString();
                        break;
                    case nameof(ValidM3USetting.Name):
                        id = service.Name;
                        break;
                }
            }

            return id;
        }

        private MxfProgram GetOrCreateProgram(string programId, string title, int epgNumber)
        {
            if (!_programsByTitle.TryGetValue(programId, out MxfProgram? existingProgram))
            {
                existingProgram = new MxfProgram(0, $"SM-{programId}")
                {
                    EPGNumber = epgNumber,
                    Title = title,
                    Description = title,
                    IsGeneric = true
                };
                _programsByTitle.TryAdd(programId, existingProgram);
            }

            return existingProgram;
        }

        private XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId, int timeShift)
        {
            MxfProgram mxfProgram = scheduleEntry.mxfProgram;
            SDSettings sdSettings = _sdSettingsMonitor.CurrentValue;

            XmltvProgramme programme = new()
            {
                Start = FormatDateTime(scheduleEntry.StartTime.AddMinutes(timeShift)),
                Stop = FormatDateTime(scheduleEntry.StartTime.AddMinutes(timeShift).AddSeconds(scheduleEntry.Duration)),
                Channel = channelId,
                Titles = ConvertToXmltvTextList(mxfProgram.Title),
                SubTitles = ConvertToXmltvTextList(mxfProgram.EpisodeTitle),
                Descriptions = ConvertToXmltvTextList(GetFullDescription(mxfProgram, scheduleEntry, sdSettings)),
                Credits = BuildProgramCredits(mxfProgram),
                Date = BuildProgramDate(mxfProgram),
                Categories = BuildProgramCategories(mxfProgram),
                Language = ConvertToXmltvText(mxfProgram.Language),
                Icons = BuildProgramIcons(mxfProgram),
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

            return programme;
        }

        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmmss +0000", CultureInfo.InvariantCulture);
        }

        private List<XmltvText>? ConvertToXmltvTextList(string? text)
        {
            return string.IsNullOrEmpty(text) ? null : [new XmltvText { Text = text }];
        }

        private XmltvText? ConvertToXmltvText(string? text)
        {
            return string.IsNullOrEmpty(text) ? null : new XmltvText { Text = text };
        }

        private string GetFullDescription(MxfProgram mxfProgram, MxfScheduleEntry scheduleEntry, SDSettings sdSettings)
        {
            string extendedDescription = sdSettings.XmltvExtendedInfoInTitleDescriptions
                ? GetExtendedDescription(mxfProgram, scheduleEntry, sdSettings)
                : string.Empty;

            return (extendedDescription + mxfProgram.Description).Trim();
        }

        private string GetExtendedDescription(MxfProgram mxfProgram, MxfScheduleEntry scheduleEntry, SDSettings sdSettings)
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

        private string GetAdvisories(MxfProgram mxfProgram)
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

        private List<string>? MxfPersonRankToXmltvCrew(List<MxfPersonRank>? mxfPersons)
        {
            return mxfPersons?.Select(person => person.Name).ToList();
        }

        private List<XmltvActor>? MxfPersonRankToXmltvActors(List<MxfPersonRank>? mxfPersons)
        {
            return mxfPersons?.Select(person => new XmltvActor { Actor = person.Name, Role = person.Character }).ToList();
        }

        private string? BuildProgramDate(MxfProgram mxfProgram)
        {
            return mxfProgram.IsMovie && mxfProgram.Year > 0
                ? mxfProgram.Year.ToString()
                : !string.IsNullOrEmpty(mxfProgram.OriginalAirdate) ? DateTime.Parse(mxfProgram.OriginalAirdate).ToString("yyyyMMdd") : null;
        }

        private List<XmltvText>? BuildProgramCategories(MxfProgram mxfProgram)
        {
            if (string.IsNullOrEmpty(mxfProgram.Keywords))
            {
                return null;
            }

            List<string> categories = [];

            foreach (string keywordId in mxfProgram.Keywords.Split(','))
            {
                if (_keywordDict.TryGetValue(keywordId, out string? word))
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

            return categories.Count == 0 ? null : categories.ConvertAll(category => new XmltvText { Text = category });
        }

        private List<XmltvIcon>? BuildProgramIcons(MxfProgram mxfProgram)
        {
            SDSettings sdSettings = _sdSettingsMonitor.CurrentValue;

            if (sdSettings.XmltvSingleImage || !mxfProgram.extras.ContainsKey("artwork"))
            {
                // Use the first available image URL
                string? url = mxfProgram.mxfGuideImage?.ImageUrl ??
                              mxfProgram.mxfSeason?.mxfGuideImage?.ImageUrl ??
                              mxfProgram.mxfSeriesInfo?.MxfGuideImage?.ImageUrl;

                return url != null ?
                [
                    new XmltvIcon
                    {
                        Src = _iconHelper.GetIconUrl(mxfProgram.EPGNumber, url, _baseUrl)
                    }
                ] : null;
            }

            // Retrieve artwork from the program, season, or series info
            List<ProgramArtwork>? artwork = mxfProgram.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                            mxfProgram.mxfSeason?.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                            mxfProgram.mxfSeriesInfo?.Extras.GetValueOrDefault("artwork") as List<ProgramArtwork>;

            // Convert artwork to XmltvIcon list
            return artwork?.Select(image => new XmltvIcon
            {
                Src = _iconHelper.GetIconUrl(mxfProgram.EPGNumber, image.Uri, _baseUrl),
                Height = image.Height,
                Width = image.Width
            }).ToList();
        }

        private List<XmltvEpisodeNum>? BuildEpisodeNumbers(MxfScheduleEntry scheduleEntry)
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

            if (mxfProgram.Series != null)
            {
                if (int.TryParse(mxfProgram.Series[2..], out int seriesIndex) &&
                    _seriesDict.TryGetValue(seriesIndex - 1, out SeriesInfo? mxfSeriesInfo) &&
                    mxfSeriesInfo.Extras.TryGetValue("tvdb", out dynamic value))
                {
                    list.Add(new XmltvEpisodeNum { System = "thetvdb.com", Text = $"series/{value}" });
                }
            }

            return list.Count > 0 ? list : null;
        }

        private XmltvVideo? BuildProgramVideo(MxfScheduleEntry scheduleEntry)
        {
            return scheduleEntry.IsHdtv ? new XmltvVideo { Quality = "HDTV" } : null;
        }

        private XmltvAudio? BuildProgramAudio(MxfScheduleEntry scheduleEntry)
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

        private XmltvPreviouslyShown? BuildProgramPreviouslyShown(MxfScheduleEntry scheduleEntry)
        {
            return scheduleEntry.IsRepeat && !scheduleEntry.mxfProgram.IsMovie ? new XmltvPreviouslyShown { Text = string.Empty } : null;
        }

        private XmltvText? BuildProgramPremiere(MxfScheduleEntry scheduleEntry)
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

        private List<XmltvSubtitles>? BuildProgramSubtitles(MxfScheduleEntry scheduleEntry)
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

        private List<XmltvRating> BuildProgramRatings(MxfScheduleEntry scheduleEntry)
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

        private void AddProgramRatingAdvisory(bool hasAdvisory, List<XmltvRating> list, string advisory)
        {
            if (hasAdvisory)
            {
                list.Add(new XmltvRating { System = "advisory", Value = advisory });
            }
        }

        private void AddProgramRating(MxfScheduleEntry scheduleEntry, List<XmltvRating> list)
        {
            HashSet<string> ratingsSet = [];

            if (scheduleEntry.extras.TryGetValue("ratings", out dynamic value))
            {
                foreach (KeyValuePair<string, string> rating in (Dictionary<string, string>)value)
                {
                    if (ratingsSet.Add(rating.Key))
                    {
                        list.Add(new XmltvRating { System = rating.Key, Value = rating.Value });
                    }
                }
            }

            if (scheduleEntry.mxfProgram.extras.TryGetValue("ratings", out dynamic valueRatings))
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

        private List<XmltvRating>? BuildProgramStarRatings(MxfProgram mxfProgram)
        {
            return mxfProgram.HalfStars == 0
                ? null
                :
                [
                    new XmltvRating { Value = $"{mxfProgram.HalfStars * 0.5:N1}/4" }
                ];
        }

        private void SortXmlTvEntries(XMLTV xmlTv)
        {
            xmlTv.Channels = xmlTv.Channels.OrderBy(c => c.Id, new NumericStringComparer()).ToList();
            xmlTv.Programs = xmlTv.Programs.OrderBy(p => p.Channel).ThenBy(p => p.StartDateTime).ToList();
        }
    }
}
