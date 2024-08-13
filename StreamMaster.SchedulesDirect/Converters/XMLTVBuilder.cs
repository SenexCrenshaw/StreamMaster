using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Comparer;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;
using StreamMaster.SchedulesDirect.Domain.Extensions;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace StreamMaster.SchedulesDirect.Converters;
public class XMLTVBuilder(IOptionsMonitor<SDSettings> intsdsettings, IOptionsMonitor<OutputProfileDict> intOutputProfileDict, IServiceProvider serviceProvider, ICustomPlayListBuilder customPlayListBuilder, IIconHelper iconHelper, IEPGHelper ePGHelper, ISchedulesDirectDataService schedulesDirectDataService, ILogger<XMLTVBuilder> logger)
    : IXMLTVBuilder
{
    private string _baseUrl = "";

    private static ConcurrentDictionary<int, MxfSeriesInfo> seriesDict = [];
    private static ConcurrentDictionary<string, string> keywordDict = [];

    private readonly ConcurrentDictionary<string, MxfProgram> _programsByTitle = [];

    private static readonly string[] tvRatings = [ "", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA",
                "", "Kinder bis 12 Jahren", "Freigabe ab 12 Jahren", "Freigabe ab 16 Jahren", "Keine Jugendfreigabe",
                "", "Déconseillé aux moins de 10 ans", "Déconseillé aux moins de 12 ans", "Déconseillé aux moins de 16 ans", "Déconseillé aux moins de 18 ans",
                "모든 연령 시청가", "7세 이상 시청가", "12세 이상 시청가", "15세 이상 시청가", "19세 이상 시청가",
                "SKY-UC", "SKY-U", "SKY-PG", "SKY-12", "SKY-15", "SKY-18", "SKY-R18" ];

    private static readonly string[] mpaaRatings = ["", "G", "PG", "PG-13", "R", "NC-17", "X", "NR", "AO"];

    public XMLTV? CreateXmlTv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs, OutputProfileDto outputProfile)
    {
        seriesDict = [];
        keywordDict = [];
        _baseUrl = baseUrl;
        try
        {

            schedulesDirectDataService.CustomStreamData().ResetLists();

            List<MxfService> toProcess = [];

            int newServiceCount = 0;

            //keywordDict = schedulesDirectDataService.AllKeywords
            //    .Where(k => !string.Equals(k.Word, "Uncategorized", StringComparison.OrdinalIgnoreCase) &&
            //    !k.Word.Contains("premiere", StringComparison.OrdinalIgnoreCase))
            //    .GroupBy(k => k.Id)
            //    .ToDictionary(
            //        g => g.Key,
            //        g =>
            //        {
            //            string word = g.First().Word;
            //            return string.Equals(word, "Movies", StringComparison.OrdinalIgnoreCase) ? "Movie" : word;
            //        }
            //    );

            keywordDict = new ConcurrentDictionary<string, string>(
                schedulesDirectDataService.AllKeywords
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
                    )
            );

            foreach (MxfSeriesInfo seriesInfo in schedulesDirectDataService.AllSeriesInfos)
            {
                _ = seriesDict.TryAdd(seriesInfo.Index, seriesInfo);
            }

            List<MxfService> services = [.. schedulesDirectDataService.AllServices.OrderBy(a => a.EPGNumber)];

            foreach (VideoStreamConfig videoStreamConfig in videoStreamConfigs.OrderBy(a => a.ChannelNumber))
            {

                MxfService? origService = services.GetMxfService(videoStreamConfig.EPGId);
                if (origService == null)
                {
                    if (videoStreamConfig.EPGId.StartsWith(EPGHelper.CustomPlayListId.ToString()))
                    {
                        (_, string title) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                        origService = schedulesDirectDataService.CustomStreamData().FindOrCreateService(videoStreamConfig.EPGId);
                        origService.EPGNumber = EPGHelper.CustomPlayListId;
                        origService.CallSign = title;
                    }
                    else

                    if (!videoStreamConfig.EPGId.StartsWith(EPGHelper.DummyId.ToString()) && !videoStreamConfig.EPGId.StartsWith(EPGHelper.SchedulesDirectId.ToString()))
                    {
                        origService = schedulesDirectDataService.DummyData().FindOrCreateService(videoStreamConfig.EPGId);
                        origService.EPGNumber = EPGHelper.DummyId;
                        origService.CallSign = videoStreamConfig.EPGId;
                    }
                    else
                    {
                        continue;
                    }

                }

                string stationId = videoStreamConfig.EPGId;
                int epgNumber = origService.EPGNumber;
                logger.LogDebug($"Processing {videoStreamConfig.Name} - {videoStreamConfig.EPGId}");

                MxfService newService = new(newServiceCount++, videoStreamConfig.EPGId);

                if (origService.MxfScheduleEntries is not null)
                {
                    newService.MxfScheduleEntries = origService.MxfScheduleEntries;
                }

                string callSign = origService.CallSign;

                if (origService.EPGNumber == EPGHelper.DummyId && !string.IsNullOrEmpty(videoStreamConfig.EPGId))
                {
                    epgNumber = EPGHelper.DummyId;
                    callSign = videoStreamConfig.EPGId;
                }

                int chNo = videoStreamConfig.ChannelNumber;

                newService.EPGNumber = epgNumber;
                newService.ChNo = chNo;
                newService.Name = videoStreamConfig.Name;
                newService.Affiliate = origService.Affiliate;
                newService.CallSign = callSign;
                newService.LogoImage = videoStreamConfig.Logo;

                newService.extras = origService.extras;
                newService.extras.AddOrUpdate("videoStreamConfig", videoStreamConfig);

                if (!string.IsNullOrEmpty(videoStreamConfig.Logo))
                {
                    newService.extras.AddOrUpdate("logo", new StationImage
                    {
                        Url = videoStreamConfig.Logo

                    });
                }
                toProcess.Add(newService);

            }

            XMLTV xmlTv = new()
            {
                Date = SMDT.UtcNow.ToString(CultureInfo.InvariantCulture),
                SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                SourceInfoName = "Stream Master",
                GeneratorInfoName = "Stream Master",
                GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                Channels = [],
                Programs = []
            };


            List<MxfProgram> programs = schedulesDirectDataService.AllPrograms;

            try
            {
                DoPrograms(toProcess, programs.Count, xmlTv, outputProfile, videoStreamConfigs);

                xmlTv.Channels = [.. xmlTv.Channels.OrderBy(a => a.Id, new NumericStringComparer())];
                xmlTv.Programs = [.. xmlTv.Programs.OrderBy(a => a.Channel).ThenBy(a => a.StartDateTime)];

            }
            catch (Exception)
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


    public XMLTV? CreateSDXmlTv(string baseUrl)
    {
        _baseUrl = baseUrl;
        try
        {
            //CreateDummyLineupChannels();

            XMLTV xmlTv = new()
            {
                Date = SMDT.UtcNow.ToString(CultureInfo.InvariantCulture),
                SourceInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                SourceInfoName = "Stream Master",
                GeneratorInfoName = "StreamMaster",
                GeneratorInfoUrl = "https://github.com/SenexCrenshaw/StreamMaster",
                Channels = [],
                Programs = []
            };


            keywordDict = new ConcurrentDictionary<string, string>(
      schedulesDirectDataService.AllKeywords
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
          )
  );

            foreach (MxfSeriesInfo seriesInfo in schedulesDirectDataService.AllSeriesInfos)
            {
                if (seriesDict.ContainsKey(seriesInfo.Index))
                {
                    MxfSeriesInfo a = seriesDict[seriesInfo.Index];

                    continue;
                }
                seriesDict.TryAdd(seriesInfo.Index, seriesInfo);
            }
            //Func<ISchedulesDirectData> data = schedulesDirectDataService.SchedulesDirectData;

            List<MxfService> services = schedulesDirectDataService.GetAllSDServices;

            List<MxfProgram> programs = schedulesDirectDataService.GetAllSDPrograms;


            OutputProfileDto profile = intOutputProfileDict.CurrentValue.GetDefaultProfileDto();

            DoPrograms(services, programs.Count, xmlTv, profile);

            xmlTv.Channels = [.. xmlTv.Channels.OrderBy(a => a.Id, new NumericStringComparer())];
            xmlTv.Programs = [.. xmlTv.Programs.Where(a => a is not null).OrderBy(a => a.Channel).ThenBy(a => a.StartDateTime)];


            return xmlTv;
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Failed to create the XMLTV file. Exception:{FileUtil.ReportExceptionMessages(ex)}");
        }
        return null;
    }

    public MxfProgram GetOrCreateProgram(int progCount, string serviceName, Movie movie, int EPGNumber)
    {
        if (!_programsByTitle.TryGetValue(movie.Id, out MxfProgram? existingProgram))
        {
            existingProgram = new(progCount + 1, $"SM-{movie.Id}")
            {
                EPGNumber = EPGNumber,
                Title = serviceName,
                Description = serviceName,
                IsGeneric = true
            };
            _programsByTitle.TryAdd(movie.Id, existingProgram);
        }

        return existingProgram;
    }
    private void DoPrograms(List<MxfService> services, int progCount, XMLTV xmlTv, OutputProfileDto outputProfile, List<VideoStreamConfig>? videoStreamConfigs = null)
    {
        _programsByTitle.Clear();
        videoStreamConfigs ??= [];

        ParallelOptions options = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        List<EPGFile> epgs = [];
        if (videoStreamConfigs is not null)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IRepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<IRepositoryContext>();

            epgs = repositoryContext.EPGFiles.ToList();
        }

        try
        {
            SDSettings sdsettings = intsdsettings.CurrentValue;
            int count = 0;

            Stopwatch sw = Stopwatch.StartNew();
            ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

            ConcurrentBag<XmltvProgramme> xmltvProgrammes = [];

            Parallel.ForEach(services, parallelOptions, service =>
            {
                XmltvChannel channel = BuildXmltvChannel(service, outputProfile);
                xmlTv.Channels.Add(channel);

                if (service.MxfScheduleEntries.ScheduleEntry.Count == 0 && sdsettings.XmltvAddFillerData)
                {
                    if (service.EPGNumber == EPGHelper.CustomPlayListId)
                    {
                        List<(Movie Movie, DateTime StartTime, DateTime EndTime)> moviesForPeriod = customPlayListBuilder.GetMoviesForPeriod(service.CallSign, SMDT.UtcNow, sdsettings.SDEPGDays);
                        foreach ((Movie movie, DateTime startTime2, DateTime endTime) in moviesForPeriod)
                        {
                            int duration = (int)(endTime - startTime2).TotalMinutes;
                            if (duration < 0)
                            {
                                continue;
                            }

                            MxfProgram program1 = GetOrCreateProgram(progCount, service.Name, movie, service.EPGNumber);

                            service.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
                            {
                                Duration = duration,
                                mxfProgram = program1,
                                StartTime = startTime2,
                                IsRepeat = true
                            });
                        }
                    }
                    else
                    {
                        MxfProgram program = new(progCount + 1, $"SM-{service.StationId}")
                        {
                            Title = service.Name,
                            Description = service.Name,
                            IsGeneric = true
                        };

                        DateTime startTime = new(SMDT.UtcNow.Year, SMDT.UtcNow.Month, SMDT.UtcNow.Day, 0, 0, 0);
                        DateTime stopTime = startTime + TimeSpan.FromDays(sdsettings.SDEPGDays);
                        do
                        {
                            service.MxfScheduleEntries.ScheduleEntry.Add(new MxfScheduleEntry
                            {
                                Duration = sdsettings.XmltvFillerProgramLength * 60 * 60,
                                mxfProgram = program,
                                StartTime = startTime,
                                IsRepeat = true
                            });
                            startTime += TimeSpan.FromHours(sdsettings.XmltvFillerProgramLength);
                        } while (startTime < stopTime);
                    }
                }

                List<MxfScheduleEntry> scheduleEntries = service.MxfScheduleEntries.ScheduleEntry;

                int timeShift = 0;
                if (videoStreamConfigs is not null)
                {
                    VideoStreamConfig? videoStreamConfig = videoStreamConfigs.FirstOrDefault(a => service.StationId == a.EPGId);
                    if (videoStreamConfig is not null)
                    {
                        if (videoStreamConfig?.TimeShift > 0)
                        {
                            timeShift = videoStreamConfig.TimeShift;
                        }
                        else
                        {
                            if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
                            {
                                (int epgNumber, string stationId) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                                if (epgNumber > 0)
                                {
                                    EPGFile? epg = epgs.FirstOrDefault(a => a.EPGNumber == epgNumber);
                                    if (epg is not null)
                                    {
                                        timeShift = epg.TimeShift;
                                    }
                                }
                            }
                        }
                    }
                }

                if (timeShift > 0)
                {
                    foreach (MxfScheduleEntry scheduleEntry in scheduleEntries)
                    {
                        scheduleEntry.StartTime = scheduleEntry.StartTime.AddMinutes(timeShift);
                    }
                }

                Parallel.ForEach(scheduleEntries, scheduleEntry =>
                {
                    XmltvProgramme program = BuildXmltvProgram(scheduleEntry, channel.Id, timeShift);
                    xmltvProgrammes.Add(program);
                    Interlocked.Increment(ref count);
                });
            });

            xmlTv.Programs.AddRange(xmltvProgrammes);

            sw.Stop();
            logger.LogInformation($"Finished processing {count} programs in {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing channels.");
        }

    }


    #region ========== XMLTV Channels and Functions ==========
    private XmltvChannel BuildXmltvChannel(MxfService mxfService, OutputProfileDto outputProfile)
    {

        string id = mxfService.CallSign;// mxfService.ChNo.ToString();
        //if (outputProfile.EnableChannelNumber)
        //{
        //    id = mxfService.ChNo.ToString();
        //}
        //if (settings.M3UUseChnoForId)
        //{
        //    id = mxfService.ChNo.ToString();
        //}

        //if (sdsettings.M3UUseCUIDForChannelID)
        //{
        //    VideoStreamConfig? vc = mxfService.extras["videoStreamConfig"] as VideoStreamConfig;
        //    id = vc.Id;
        //    //id = mxfService.ChNo.ToString();
        //}

        // initialize the return channel
        XmltvChannel ret = new()
        {
            Id = id,
            DisplayNames = []
        };

        //ret.DisplayNames.Add(new XmltvText { Text = id });
        ret.DisplayNames.Add(new XmltvText { Text = mxfService.CallSign });
        if (mxfService.Name == null)
        {
            mxfService.Name = mxfService.CallSign;
            ret.DisplayNames.Add(new XmltvText { Text = mxfService.Name });
        }
        else
        {
            if (!mxfService.Name.Equals(mxfService.CallSign))
            {
                ret.DisplayNames.Add(new XmltvText { Text = mxfService.Name });
            }
        }
        SDSettings sdsettings = intsdsettings.CurrentValue;
        // add channel number if requested
        if (sdsettings.XmltvIncludeChannelNumbers)
        {
            List<string> numbers = [];

            foreach (MxfLineup mxfLineup in schedulesDirectDataService.AllLineups)
            {
                foreach (MxfChannel mxfChannel in mxfLineup.channels)
                {
                    if (mxfChannel.Service != mxfService.Id || mxfChannel.Number <= 0)
                    {
                        continue;
                    }

                    string num = $"{mxfChannel.Number}" + (mxfChannel.SubNumber > 0 ? $".{mxfChannel.SubNumber}" : "");

                    if (numbers.Contains(num))
                    {
                        continue;
                    }
                    numbers.Add(num);
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

        // add logo if available
        if (mxfService.extras.TryGetValue("logo", out _))
        {
            ret.Icons =
                [
                    new() {
                        Src = iconHelper.GetIconUrl(mxfService.EPGNumber,  mxfService.extras["logo"].Url,_baseUrl),
                        Height = mxfService.extras["logo"].Height,
                        Width = mxfService.extras["logo"].Width
                    }
                ];

        }
        return ret;
    }
    #endregion

    #region ========== XMLTV Programmes and Functions ==========

    //[LogExecutionTimeAspect]
    public XmltvProgramme BuildXmltvProgram(MxfScheduleEntry scheduleEntry, string channelId, int timeShift)
    {

        MxfProgram mxfProgram = scheduleEntry.mxfProgram;
        SDSettings sdsettings = intsdsettings.CurrentValue;
        string descriptionExtended = sdsettings.XmltvExtendedInfoInTitleDescriptions
                                                  ? GetDescriptionExtended(mxfProgram, scheduleEntry, sdsettings)
                                                  : string.Empty;

        if (scheduleEntry.XmltvProgramme != null)
        {
            if (timeShift > 0)
            {
                scheduleEntry.XmltvProgramme.Start = $"{scheduleEntry.XmltvProgramme.StartDateTime.AddHours(timeShift):yyyyMMddHHmmss} +0000";
                scheduleEntry.XmltvProgramme.Stop = $"{scheduleEntry.XmltvProgramme.StartDateTime.AddHours(timeShift) + TimeSpan.FromSeconds(scheduleEntry.Duration):yyyyMMddHHmmss} +0000";
            }

            scheduleEntry.XmltvProgramme.Channel = channelId;
            scheduleEntry.XmltvProgramme.Descriptions = MxfStringToXmlTextArray((descriptionExtended + mxfProgram.Description).Trim());
            scheduleEntry.XmltvProgramme.Icons = BuildProgramIcons(mxfProgram);
            //scheduleEntry.XmltvProgramme.Categories = BuildProgramCategories(mxfProgram);
            //    //scheduleEntry.XmltvProgramme.StarRating = BuildProgramStarRatings(mxfProgram);
            //    //scheduleEntry.XmltvProgramme.Rating = BuildProgramRatings(scheduleEntry);
            //    //scheduleEntry.XmltvProgramme.StarRating = BuildProgramStarRatings(mxfProgram);
            //    //return scheduleEntry.XmltvProgramme;
            return scheduleEntry.XmltvProgramme;
        }

        if (mxfProgram.ProgramId.StartsWith($"SM-"))
        {
            string movieIdString = mxfProgram.ProgramId[3..];

            (CustomPlayList? customPlayList, CustomStreamNfo? customStreamNfo) = customPlayListBuilder.GetCustomPlayListByMovieId(movieIdString);

            if (customPlayList is not null && customStreamNfo is not null)
            {
                XmltvProgramme test = XmltvProgrammeConverter.ConvertMovieToXmltvProgramme(customStreamNfo.Movie);
                test.Channel = channelId;
                test.Start = scheduleEntry.StartTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                test.Stop = scheduleEntry.StartTime.AddMinutes(scheduleEntry.Duration).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);


                mxfProgram.Description = customStreamNfo.Movie.Plot;


                string directory = Path.GetDirectoryName(customStreamNfo.VideoFileName);
                string fanArt = Path.Combine(directory, "poster.jpg");
                string artUrl = fanArt.Replace(BuildInfo.CustomPlayListFolder, "");
                string encodedFilePath = Uri.EscapeDataString(artUrl);
                string IconSource = $"{_baseUrl}/api/files/{(int)SMFileTypes.CustomPlayListArt}/{encodedFilePath}";

                test.Icons = File.Exists(fanArt)
                    ? ([new XmltvIcon {
                        Src =IconSource
                    }])
                    : null;


                mxfProgram.Title = customStreamNfo.Movie.Title;

                return test;
            }
        }

        XmltvProgramme ret = new()
        {
            Start = $"{scheduleEntry.StartTime.AddHours(timeShift):yyyyMMddHHmmss} +0000",
            Stop = $"{scheduleEntry.StartTime.AddHours(timeShift) + TimeSpan.FromSeconds(scheduleEntry.Duration):yyyyMMddHHmmss} +0000",
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
        return ret;
    }

    private static string GetDescriptionExtended(MxfProgram mxfProgram, MxfScheduleEntry scheduleEntry, SDSettings sdsettings)
    {

        string descriptionExtended = "";
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

            if (!sdsettings.PrefixEpisodeTitle && !sdsettings.PrefixEpisodeDescription && !sdsettings.AppendEpisodeDesc)
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
            descriptionExtended = descriptionExtended.Trim() + "\u000D\u000A";
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

    // Titles, SubTitles, and Descriptions
    private static List<XmltvText>? MxfStringToXmlTextArray(string mxfString)
    {
        return string.IsNullOrEmpty(mxfString) ? null : [new() { Text = mxfString }];
    }

    // Credits

    private static XmltvCredit? BuildProgramCredits(MxfProgram mxfProgram)
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

    private static List<XmltvText>? BuildProgramCategories(MxfProgram mxfProgram)
    {
        if (string.IsNullOrEmpty(mxfProgram.Keywords))
        {
            return null;
        }

        List<string> categories = [];

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
            _ = categories.Remove("Movies");
            if (!categories.Contains("Movie"))
            {
                categories.Add("Movie");
            }
        }

        // Remove "Kids" if "Children" is also present
        if (categories.Contains("Children"))
        {
            _ = categories.Remove("Kids");
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
        SDSettings sdsettings = intsdsettings.CurrentValue;

        if (sdsettings.XmltvSingleImage || !mxfProgram.extras.ContainsKey("artwork"))
        {
            // Use the first available image URL
            string? url = mxfProgram.mxfGuideImage?.ImageUrl ??
                          mxfProgram.mxfSeason?.mxfGuideImage?.ImageUrl ??
                          mxfProgram.mxfSeriesInfo?.mxfGuideImage?.ImageUrl;

            return url != null ? [new XmltvIcon {
                Src = iconHelper.GetIconUrl(mxfProgram.EPGNumber, url,  _baseUrl)
            }] : null;
        }

        // Retrieve artwork from the program, season, or series info
        List<ProgramArtwork>? artwork = mxfProgram.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                       mxfProgram.mxfSeason?.extras.GetValueOrDefault("artwork") as List<ProgramArtwork> ??
                                       mxfProgram.mxfSeriesInfo?.extras.GetValueOrDefault("artwork") as List<ProgramArtwork>;

        // Convert artwork to XmltvIcon list
        return artwork?.Select(image => new XmltvIcon
        {
            Src = iconHelper.GetIconUrl(mxfProgram.EPGNumber, image.Uri, _baseUrl),
            Height = image.Height,
            Width = image.Width
        }).ToList();
    }

    private static XmltvText? GrabSportEvent(MxfProgram program)
    {
        return !program.IsSports || !program.extras.TryGetValue("genres", out dynamic? value)
            ? null
            : (from category in (string[])value where !category.StartsWith("sport", StringComparison.OrdinalIgnoreCase) select new XmltvText { Text = category }).FirstOrDefault();
    }


    private static List<XmltvText>? BuildSportTeams(MxfProgram program)
    {
        return !program.IsSports || !program.extras.TryGetValue("teams", out dynamic? value)
            ? (List<XmltvText>?)null
            : ((List<string>)value).Select(team => new XmltvText { Text = team }).ToList();
    }

    // EpisodeNums

    private static readonly Random RandomNumber = new();

    private static List<XmltvEpisodeNum>? BuildEpisodeNumbers(MxfScheduleEntry mxfScheduleEntry)
    {
        List<XmltvEpisodeNum> list = [];
        MxfProgram mxfProgram = mxfScheduleEntry.mxfProgram;


        if (mxfProgram.EpisodeNumber != 0 || mxfScheduleEntry.Part != 0)
        {
            string seasonPart = mxfProgram.SeasonNumber > 0 ? (mxfProgram.SeasonNumber - 1).ToString() : "";
            string episodePart = mxfProgram.EpisodeNumber > 0 ? (mxfProgram.EpisodeNumber - 1).ToString() : "";
            string part = mxfScheduleEntry.Part > 0 ? $"{mxfScheduleEntry.Part - 1}/" : "0/";
            string parts = mxfScheduleEntry.Parts > 0 ? mxfScheduleEntry.Parts.ToString() : "1";
            string text = $"{seasonPart}.{episodePart}.{part}{parts}";

            list.Add(new XmltvEpisodeNum { System = "xmltv_ns", Text = text });
        }
        else if (mxfProgram.EPGNumber is EPGHelper.DummyId or EPGHelper.CustomPlayListId)
        {
            list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd}" });
        }
        else if (!mxfProgram.ProgramId.StartsWith("MV"))
        {
            if (string.IsNullOrEmpty(mxfProgram.OriginalAirdate))
            {
                list.Add(new XmltvEpisodeNum { System = "original-air-date", Text = $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd}" });
            }
            else
            {
                string oad = mxfProgram.OriginalAirdate;

                oad = !mxfScheduleEntry.IsRepeat
                    ? $"{mxfScheduleEntry.StartTime.ToLocalTime():yyyy-MM-dd HH:mm}:{RandomNumber.Next(1, 60):00}"
                    : $"{DateTime.Parse(oad):yyyy-MM-dd}";

                list.Add(new XmltvEpisodeNum
                {
                    System = "original-air-date",
                    Text = oad
                }
                );

            }
        }

        if (mxfProgram.Series != null)
        {
            if (seriesDict.TryGetValue(int.Parse(mxfProgram.Series[2..]) - 1, out MxfSeriesInfo? mxfSeriesInfo) &&
                mxfSeriesInfo.extras.TryGetValue("tvdb", out dynamic? value))
            {
                list.Add(new XmltvEpisodeNum { System = "thetvdb.com", Text = $"series/{value}" });
            }
        }
        return list.Count > 0 ? list : null;
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
        List<string> hashSet = [];
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
