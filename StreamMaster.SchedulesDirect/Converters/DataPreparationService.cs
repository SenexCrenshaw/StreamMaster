using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;

namespace StreamMaster.SchedulesDirect.Converters
{
    public class DataPreparationService(
        IOptionsMonitor<SDSettings> sdSettingsMonitor,
        IOptionsMonitor<OutputProfileDict> outputProfileDictMonitor,
        IServiceProvider serviceProvider,
        ICustomPlayListBuilder customPlayListBuilder,
        ISchedulesDirectDataService schedulesDirectDataService) : IDataPreparationService
    {
        public string BaseUrl { get; private set; } = string.Empty;

        private readonly ConcurrentDictionary<string, MxfProgram> _programsByTitle = new();
        private readonly ConcurrentDictionary<int, SeriesInfo> _seriesDict = new();
        private ConcurrentDictionary<string, string> _keywordDict = new();
        public IReadOnlyDictionary<string, string> KeywordDict => _keywordDict;
        public IReadOnlyDictionary<int, SeriesInfo> SeriesDict => _seriesDict;

        public void Initialize(string baseUrl, List<VideoStreamConfig>? videoStreamConfigs)
        {
            BaseUrl = baseUrl;

            InitializeDataDictionaries();

            if (videoStreamConfigs != null)
            {
                schedulesDirectDataService.CustomStreamData().ResetLists();
            }
        }

        private void InitializeDataDictionaries()
        {
            _seriesDict.Clear();
            _keywordDict.Clear();

            foreach (SeriesInfo seriesInfo in schedulesDirectDataService.AllSeriesInfos)
            {
                _ = _seriesDict.TryAdd(seriesInfo.Index, seriesInfo);
            }

            _keywordDict = new ConcurrentDictionary<string, string>(
                schedulesDirectDataService.AllKeywords
                    .Where(k => !k.Word.EndsWithIgnoreCase("Uncategorized")
                                && !k.Word.ContainsIgnoreCase("premiere"))
                    .GroupBy(k => k.Id)
                    .ToDictionary(
                        g => g.Key,
                        g =>
                        {
                            string word = g.First().Word;
                            return word.EqualsIgnoreCase("Movies") ? "Movie" : word;
                        }
                    )
            );
        }

        public List<MxfService> GetServicesToProcess(List<VideoStreamConfig> videoStreamConfigs)
        {
            List<MxfService> servicesToProcess = [];
            List<MxfService> allServices = [.. schedulesDirectDataService.AllServices.OrderBy(a => a.EPGNumber)];
            int newServiceCount = 0;

            foreach (VideoStreamConfig? videoStreamConfig in videoStreamConfigs.OrderBy(a => a.ChannelNumber))
            {
                MxfService? origService = FindOriginalService(videoStreamConfig, allServices);

                if (origService == null)
                {
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

        public List<MxfService> GetAllSdServices()
        {
            return schedulesDirectDataService.GetAllSDServices;
        }

        public OutputProfileDto GetDefaultOutputProfile()
        {
            return outputProfileDictMonitor.CurrentValue.GetDefaultProfileDto();
        }

        public SDSettings GetSdSettings()
        {
            return sdSettingsMonitor.CurrentValue;
        }

        public List<EPGFile> GetEpgFiles(List<VideoStreamConfig> videoStreamConfigs)
        {
            List<EPGFile> epgFiles = [];

            if (videoStreamConfigs.Count != 0)
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                IRepositoryContext repositoryContext = scope.ServiceProvider.GetRequiredService<IRepositoryContext>();
                epgFiles = [.. repositoryContext.EPGFiles];
            }

            return epgFiles;
        }

        private static MxfService? FindOriginalService(VideoStreamConfig videoStreamConfig, List<MxfService> allServices)
        {
            MxfService? origService = allServices.Find(s => s.StationId == videoStreamConfig.EPGId);
            origService ??= allServices.Find(s => s.StationId.EndsWith("-" + videoStreamConfig.EPGId));
            return origService;
        }

        private MxfService? HandleMissingOriginalService(VideoStreamConfig videoStreamConfig)
        {
            if (videoStreamConfig.EPGId.StartsWith(EPGHelper.CustomPlayListId.ToString()))
            {
                string callsign = videoStreamConfig.EPGId;
                if (EPGHelper.IsValidEPGId(videoStreamConfig.EPGId))
                {
                    (_, callsign) = videoStreamConfig.EPGId.ExtractEPGNumberAndStationId();
                }
                MxfService origService = schedulesDirectDataService.CustomStreamData().FindOrCreateService(videoStreamConfig.EPGId);
                origService.EPGNumber = EPGHelper.CustomPlayListId;
                origService.CallSign = callsign;
                return origService;
            }
            else if (!videoStreamConfig.EPGId.StartsWith(EPGHelper.DummyId.ToString()) && !videoStreamConfig.EPGId.StartsWith(EPGHelper.SchedulesDirectId.ToString()))
            {
                MxfService origService = schedulesDirectDataService.DummyData().FindOrCreateService(videoStreamConfig.EPGId);
                origService.EPGNumber = EPGHelper.DummyId;
                origService.CallSign = videoStreamConfig.EPGId;
                return origService;
            }
            else
            {
                return null;
            }
        }

        private static MxfService CreateNewService(VideoStreamConfig videoStreamConfig, MxfService origService, int serviceId)
        {
            MxfService newService = new(serviceId, videoStreamConfig.EPGId)
            {
                EPGNumber = origService.EPGNumber,
                ChNo = videoStreamConfig.ChannelNumber,
                Name = videoStreamConfig.Name,
                Affiliate = origService.Affiliate,
                CallSign = origService.CallSign,
                //LogoImage = videoStreamConfig.Logo,
                extras = new ConcurrentDictionary<string, dynamic>(origService.extras),
                MxfScheduleEntries = origService.MxfScheduleEntries
            };

            newService.extras.AddOrUpdate("videoStreamConfig", videoStreamConfig, (_, _) => videoStreamConfig);

            if (!string.IsNullOrEmpty(videoStreamConfig.Logo))
            {
                newService.extras.AddOrUpdate("logo", new StationImage
                {
                    Url = videoStreamConfig.Logo
                }, (_, _) => new StationImage { Url = videoStreamConfig.Logo });
            }

            return newService;
        }

        public void AdjustServiceSchedules(MxfService service)
        {
            SDSettings sdSettings = GetSdSettings();

            if (service.MxfScheduleEntries.ScheduleEntry.Count == 0 && sdSettings.XmltvAddFillerData)
            {
                if (service.EPGNumber == EPGHelper.CustomPlayListId)
                {
                    // Custom playlist logic
                    List<(Movie Movie, DateTime StartTime, DateTime EndTime)> moviesForPeriod = customPlayListBuilder.GetMoviesForPeriod(service.CallSign, DateTime.UtcNow, sdSettings.SDEPGDays);

                    foreach ((Movie movie, DateTime startTime, DateTime endTime) in moviesForPeriod)
                    {
                        if (movie.Id == null)
                        {
                            continue;
                        }
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
                    // Filler data for other services DUMMY
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

        public int GetTimeShift(VideoStreamConfig? videoStreamConfig, List<EPGFile> epgFiles)
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
                _ = _programsByTitle.TryAdd(programId, existingProgram);
            }

            return existingProgram;
        }
    }
}