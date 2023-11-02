using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using StreamMasterApplication.Common.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using StreamMasterDomain.Services;

namespace StreamMaster.SchedulesDirectAPI.Services;


[LogExecutionTimeAspect]
public class SDService(IMemoryCache memoryCache, ILogger<SDService> logger, ISettingsService settingsService, ISchedulesDirect sd) : ISDService
{
    //private readonly SchedulesDirect sd;

    //private async Task<bool> EnsureSDAsync(CancellationToken cancellationToken = default)
    //{
    //    if (sd != null)
    //    {
    //        return true;
    //    }

    //    Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
    //    if (!setting.SDEnabled)
    //    {
    //        return false;
    //    }

    //    sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
    //    if (sd is not null)
    //    {

    //        await sd.Sync(setting.SDStationIds, cancellationToken);
    //    }

    //    return sd != null;
    //}
    [LogExecutionTimeAspect]
    public async Task SDSync(CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return;
        //}

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return;
        }

        List<SDProgram> programs = await sd.Sync(setting.SDStationIds, cancellationToken);
        memoryCache.SetSDProgreammesCache(programs);
    }

    [LogExecutionTimeAspect]
    public async Task<List<Programme>> GetProgrammes(int maxDays, int maxRatings, bool useLineUpInName, CancellationToken cancellationToken)
    {
        if (memoryCache.SDProgrammess()?.Any() != true)
        {
            System.Runtime.CompilerServices.ConfiguredTaskAwaitable<Setting> settingTask = settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
            System.Runtime.CompilerServices.ConfiguredTaskAwaitable<List<Station>> stationsTask = GetStations(cancellationToken).ConfigureAwait(false);

            Setting setting = await settingTask;
            List<Station> stations = await stationsTask;

            HashSet<string> stationsIds = setting.SDStationIds.Select(a => a.StationId).ToHashSet();
            Dictionary<string, Station> stationDictionary = stations.ToDictionary(s => s.StationId);

            List<ChannelLogoDto> channelLogos = memoryCache.ChannelLogos();
            int nextId = channelLogos.Any() ? channelLogos.Max(a => a.Id) + 1 : 0;

            // Process logos in parallel
            List<ChannelLogoDto> logoTasks = setting.SDStationIds.AsParallel().WithCancellation(cancellationToken).SelectMany(SDStationId =>
            {
                if (stationDictionary.TryGetValue(SDStationId.StationId, out Station? station) && station.LineUp == SDStationId.LineUp)
                {
                    return station.StationLogo?.Where(logo => !channelLogos.Any(a => a.LogoUrl == logo.URL))
                        .Select(logo => new ChannelLogoDto
                        {
                            Id = Interlocked.Increment(ref nextId),
                            LogoUrl = logo.URL,
                            EPGId = "SD|" + SDStationId.StationId,
                            EPGFileId = 0
                        }) ?? Enumerable.Empty<ChannelLogoDto>();
                }
                return Enumerable.Empty<ChannelLogoDto>();
            }).ToList();

            logoTasks.ForEach(logo => memoryCache.Add(logo));

            List<Schedule>? schedules = await GetSchedules(stationsIds.ToList(), cancellationToken).ConfigureAwait(false);
            if (schedules == null || !schedules.Any())
            {
                return new List<Programme>();
            }

            List<Programme> programmes = new();
            DateTime now = DateTime.Now;
            DateTime maxDate = now.AddDays(maxDays);

            foreach (Schedule sched in schedules)
            {
                if (!stationDictionary.TryGetValue(sched.StationID, out Station? station))
                {
                    continue;
                }

                string channelNameSuffix = station.Name ?? sched.StationID;
                string displayName = useLineUpInName ? $"{station.LineUp}-{channelNameSuffix}" : channelNameSuffix;
                string channelName = $"SD - {channelNameSuffix}";

                List<Program> relevantPrograms = sched.Programs.Where(p => p.AirDateTime <= maxDate).ToList();
                List<string> progIds = relevantPrograms.Select(p => p.ProgramID).Distinct().ToList();

                List<SDProgram> sdPrograms = await sd.GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
                Dictionary<string, SDProgram> sdProgramsDict = sdPrograms.ToDictionary(p => p.ProgramID);

                foreach (Program? p in relevantPrograms)
                {
                    if (!sdProgramsDict.TryGetValue(p.ProgramID, out SDProgram? sdProg))
                    {
                        continue;
                    }

                    DateTime startt = p.AirDateTime;
                    DateTime endt = startt.AddSeconds(p.Duration);
                    string lang = station.BroadcastLanguage.FirstOrDefault() ?? "en";

                    Programme programme = new()
                    {
                        Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
                        Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
                        Channel = $"SD|{sched.StationID}",
                        ChannelName = channelName,
                        Name = channelNameSuffix,
                        DisplayName = displayName,
                        Title = SchedulesDirect.GetTitles(sdProg.Titles, lang),
                        Subtitle = SchedulesDirect.GetSubTitles(sdProg, lang),
                        Desc = SchedulesDirect.GetDescriptions(sdProg, lang),
                        Credits = SchedulesDirect.GetCredits(sdProg, lang),
                        Category = SchedulesDirect.GetCategory(sdProg, lang),
                        Language = lang,
                        Episodenum = SchedulesDirect.GetEpisodeNums(sdProg, lang),
                        Icon = SchedulesDirect.GetIcons(p, sdProg, sched, lang),
                        Rating = SchedulesDirect.GetRatings(sdProg, lang, maxRatings),
                        Video = SchedulesDirect.GetTvVideos(p),
                        Audio = SchedulesDirect.GetTvAudios(p),
                    };

                    programmes.Add(programme);
                }
            }

            memoryCache.SetSDProgreammesCache(programmes);
            return programmes;
        }
        else
        {
            return memoryCache.SDProgrammess();
        }
    }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        return await sd.GetCountries(cancellationToken);
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return new();
        //}

        return await sd.GetStationPreviews(cancellationToken);
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return new();
        //}

        return await sd.GetSchedules(stationsIds, cancellationToken);
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return new();
        //}
        return await sd.GetSDPrograms(progIds, cancellationToken);
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return new();
        //}
        return await sd.GetStations(cancellationToken);
    }

    public async Task<SDStatus> GetStatus(CancellationToken cancellationToken)
    {
        //if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        //{
        //    return SDHelpers.GetSDStatusOffline();
        //}

        return await sd.GetStatus(cancellationToken);
    }

    public async Task<string> GetEpg(CancellationToken cancellationToken)
    {
        return await sd.GetEpg(cancellationToken);
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        return await sd.GetHeadends(country, postalCode, cancellationToken);
    }
    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {
        return await sd.GetLineup(lineUp, cancellationToken);
    }

    public async Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken)
    {
        return await sd.GetLineUpPreviews(cancellationToken);
    }

    public async Task<List<Lineup>> GetLineups(CancellationToken cancellationToken)
    {
        return await sd.GetLineups(cancellationToken);
    }
}
