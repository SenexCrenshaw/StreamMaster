using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Helpers;


using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Services;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDService(IMemoryCache memoryCache, ILogger<SDService> logger, ISettingsService settingsService) : ISDService
{
    private SchedulesDirect sd;

    private async Task<bool> EnsureSDAsync(CancellationToken cancellationToken = default)
    {
        if (sd != null)
        {
            return true;
        }

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }

        sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        if (sd is not null)
        {

            await sd.Sync(setting.SDStationIds, cancellationToken);
        }

        return sd != null;
    }


    public async Task<List<Programme>> GetProgrammes(int maxDays, int maxRatings, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

        //List<string> stationsIds = new();// setting.SDStationIds.Select(a => a.StationId).Distinct().ToList();
        HashSet<string> stationsIds = new();

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        foreach (StationIdLineUp SDStationId in setting.SDStationIds)
        {
            stationsIds.Add(SDStationId.StationId);
            string stationId = SDStationId.StationId;
            string lineUp = SDStationId.LineUp;

            List<string> names = stations.Where(a => a.StationId == stationId && a.LineUp == lineUp).Select(a => a.Name).Distinct().ToList();
            List<StationLogo> logos = stations.Where(a => a.StationId == stationId && a.StationLogo != null && a.LineUp == lineUp).SelectMany(a => a.StationLogo).Distinct().ToList();
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
                        //EPGId = "SD|" + lineUp + "|" + stationId,
                        EPGId = "SD|" + stationId,
                        EPGFileId = 0
                    };

                    memoryCache.Add(cl);
                }

            }
        }


        List<Schedule>? schedules = await GetSchedules(stationsIds.ToList(), cancellationToken);

        if (schedules?.Any() != true)
        {
            return new();
        }

        List<Programme> retProgrammes = new();
        foreach (Schedule sched in schedules)
        {
            IStation station = stations.First(a => a.StationId == sched.StationID);
            List<string> names = stations.Where(a => a.StationId == sched.StationID).Select(a => a.Name).Distinct().ToList();
            string? channelNameSuffix = names.LastOrDefault();
            string displayName = "";
            string channelName = "";
            string name = "";

            if (channelNameSuffix != null && channelNameSuffix != sched.StationID)
            {
                displayName = station.LineUp + "-" + channelNameSuffix;
                channelName = station.LineUp + "-" + sched.StationID + " - " + channelNameSuffix;
                name = channelNameSuffix;
            }
            else
            {
                displayName = station.LineUp + "-" + sched.StationID;
                channelName = sched.StationID;
                name = sched.StationID;
            }

            List<string> progIds = sched.Programs.Where(a => a.AirDateTime <= DateTime.Now.AddDays(maxDays)).Select(a => a.ProgramID).Distinct().ToList();

            List<SDProgram> programs = await sd.GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);


            foreach (Program p in sched.Programs)
            {
                //foreach (SDProgram sdProg in programs)
                SDProgram? sdProg = programs.FirstOrDefault(a => a.ProgramID == p.ProgramID);
                if (sdProg == null)
                {
                    continue;
                }

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
                    //Channel = "SD|" + station.LineUp + "|" + sched.StationID,
                    Channel = "SD|" + sched.StationID,
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
                    Rating = SchedulesDirect.GetRatings(sdProg, lang, maxRatings),
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
            //var s= GetSubTtitle()
        }
        return retProgrammes;
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        {
            return new();
        }

        return await sd.GetStationPreviews(cancellationToken);
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken)
    {
        if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        {
            return new();
        }

        return await sd.GetSchedules(stationsIds, cancellationToken);
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken)
    {
        if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        {
            return new();
        }
        return await sd.GetSDPrograms(progIds, cancellationToken);
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        {
            return new();
        }
        return await sd.GetStations(cancellationToken);
    }

    public async Task<SDStatus> GetStatus(CancellationToken cancellationToken)
    {
        if (!await EnsureSDAsync(cancellationToken).ConfigureAwait(false))
        {
            return SDHelpers.GetSDStatusOffline();
        }

        return await sd.GetStatus(cancellationToken);
    }
}
