using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

using StreamMasterApplication.Services;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.EPG;
using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services;

public class SDService(IMemoryCache memoryCache, ILogger<SDService> logger, ISettingsService settingsService) : ISDService
{
    private SchedulesDirect? sd = null;

    private async Task GetSD()
    {
        Setting setting = await settingsService.GetSettingsAsync();
        sd ??= new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
    }


    public async Task<List<Programme>> GetProgrammes(CancellationToken cancellationToken)
    {
        List<Programme> ret = memoryCache.SDProgrammess();
        if (ret.Any())
        {
            return ret;
        }

        Setting setting = await settingsService.GetSettingsAsync();
        List<string> stationsIds = setting.SDStationIds.Select(a => a.StationId).Distinct().ToList();

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        foreach (string? stationId in stationsIds)
        {
            List<string> names = stations.Where(a => a.StationID == stationId).Select(a => a.Name).Distinct().ToList();
            List<StationLogo> logos = stations.Where(a => a.StationID == stationId && a.StationLogo != null).SelectMany(a => a.StationLogo).Distinct().ToList();
            List<ChannelLogoDto> channelLogos = memoryCache.ChannelLogos();


            int nextId = 0;

            if (channelLogos.Any())
            {
                nextId = channelLogos.Max(a => a.Id);
            }
            foreach (StationLogo? logo in logos)
            {
                if (!channelLogos.Any(a => a.LogoUrl == logo.URL))
                {
                    ChannelLogoDto cl = new()
                    {
                        Id = nextId++,
                        LogoUrl = logo.URL,
                        EPGId = "SD-" + stationId,
                        EPGFileId = 0
                    };

                    memoryCache.Add(cl);
                }

            }
        }


        List<Schedule>? schedules = await GetSchedules(stationsIds, cancellationToken);

        if (schedules == null || !schedules.Any())
        {
            return new();
        }

        List<string> progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
        List<SDProgram> programs = await sd.GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);
        List<Programme> retProgrammes = new();

        foreach (SDProgram sdProg in programs)
        {
            List<Schedule> scheds = schedules.Where(a => a.Programs.Any(a => a.ProgramID == sdProg.ProgramID)).ToList();

            foreach (Schedule sched in scheds)
            {
                Station station = stations.Where(a => a.StationID == sched.StationID).First();
                List<string> names = stations.Where(a => a.StationID == sched.StationID).Select(a => a.Name).Distinct().ToList();

                string channelNameSuffix = names.LastOrDefault();
                string displayName = "";
                string channelName = "";
                string name = "";

                if (channelNameSuffix != null && channelNameSuffix != sched.StationID)
                {
                    displayName = "SD : " + channelNameSuffix;
                    channelName = sched.StationID + " - " + channelNameSuffix;
                    name = channelNameSuffix;
                }
                else
                {
                    displayName = "SD : " + sched.StationID;
                    channelName = sched.StationID;
                    name = sched.StationID;
                }

                foreach (Program p in sched.Programs)
                {
                    DateTime startt = p.AirDateTime;
                    DateTime endt = startt.AddSeconds(p.Duration);
                    string lang = "en";
                    if (station.BroadcastLanguage.Any())
                    {
                        lang = station.BroadcastLanguage[0];
                    }

                    Programme programme = new()
                    {
                        Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
                        Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
                        Channel = "SD-" + sched.StationID,
                        ChannelName = channelName,
                        Name = name,
                        DisplayName = displayName,
                        Title = SchedulesDirect.GetTitles(sdProg.Titles, lang),
                        Subtitle = SchedulesDirect.GetSubTitles(sdProg, lang),
                        Desc = SchedulesDirect.GetDescriptions(sdProg, lang),
                        Credits = SchedulesDirect.GetCredits(sdProg, lang),
                        Category = SchedulesDirect.GetCategory(sdProg, lang),
                        Language = lang,
                        Episodenum = SchedulesDirect.GetEpisodeNums(sdProg, lang),
                        Icon = SchedulesDirect.GetIcons(p, sdProg, sched, lang),
                        Rating = SchedulesDirect.GetRatings(sdProg, lang),
                        Video = SchedulesDirect.GetTvVideos(p),
                        Audio = SchedulesDirect.GetTvAudios(p),

                    };

                    if (p.New is not null)
                    {
                        programme.New = ((bool)p.New).ToString();
                    }

                    if (!string.IsNullOrEmpty(p.LiveTapeDelay) && p.LiveTapeDelay.Equals("Live"))
                    {
                        programme.Live = "";
                    }

                    retProgrammes.Add(programme);
                }
            }
            //var s= GetSubTtitle()
        }

        memoryCache.SetSDProgreammesCache(retProgrammes, TimeSpan.FromHours(4));



        return retProgrammes;
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        await GetSD();
        return await sd.GetStationPreviews(cancellationToken);
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken)
    {
        await GetSD();
        return await sd.GetSchedules(stationsIds, cancellationToken);
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken)
    {
        await GetSD();
        return await sd.GetSDPrograms(progIds, cancellationToken);
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        await GetSD();
        return await sd.GetStations(cancellationToken);
    }
}
