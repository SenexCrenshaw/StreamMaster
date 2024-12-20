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
        private readonly ConcurrentDictionary<string, MxfProgram> _programsByTitle = new();

        public void Initialize(string baseUrl, List<VideoStreamConfig>? videoStreamConfigs)
        {
            if (videoStreamConfigs != null)
            {
                schedulesDirectDataService.CustomStreamData().ResetLists();
            }
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