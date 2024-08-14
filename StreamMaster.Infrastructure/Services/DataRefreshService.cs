using Microsoft.AspNetCore.SignalR;

using StreamMaster.Application.Interfaces;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services;

public partial class DataRefreshService(IHubContext<StreamMasterHub, IStreamMasterHub> hub) : IDataRefreshService, IDataRefreshServicePartial
{

    public async Task RefreshAll()
    {

        await RefreshChannelGroups(true);
        await RefreshCustom(true);
        await RefreshEPG(true);
        await RefreshEPGFiles(true);
        await RefreshGeneral(true);
        await RefreshIcons(true);
        await RefreshM3UFiles(true);
        await RefreshProfiles(true);
        await RefreshSchedulesDirect(true);
        await RefreshSettings(true);
        await RefreshSMChannels(true);
        await RefreshSMChannelStreamLinks(true);
        await RefreshSMStreams(true);
        await RefreshSMTasks(true);
        await RefreshStatistics(true);
        await RefreshStreamGroups(true);
        await RefreshStreamGroupSMChannelLinks(true);
        await RefreshVs(true);
    }

    public async Task RefreshChannelGroups(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetChannelGroups");
        await hub.Clients.All.DataRefresh("GetChannelGroupsFromSMChannels");
        await hub.Clients.All.DataRefresh("GetPagedChannelGroups");
    }

    public async Task RefreshCustom(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetCustomPlayLists");
        await hub.Clients.All.DataRefresh("GetIntroPlayLists");
    }

    public async Task RefreshEPG(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetEPGColors");
    }

    public async Task RefreshEPGFiles(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetEPGFiles");
        await hub.Clients.All.DataRefresh("GetEPGNextEPGNumber");
        await hub.Clients.All.DataRefresh("GetPagedEPGFiles");
    }

    public async Task RefreshGeneral(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetDownloadServiceStatus");
        await hub.Clients.All.DataRefresh("GetIsSystemReady");
        await hub.Clients.All.DataRefresh("GetSystemStatus");
        await hub.Clients.All.DataRefresh("GetTaskIsRunning");
    }

    public async Task RefreshIcons(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetIcons");
    }

    public async Task RefreshM3UFiles(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetM3UFileNames");
        await hub.Clients.All.DataRefresh("GetM3UFiles");
        await hub.Clients.All.DataRefresh("GetPagedM3UFiles");
    }

    public async Task RefreshProfiles(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetCommandProfiles");
        await hub.Clients.All.DataRefresh("GetOutputProfiles");
    }

    public async Task RefreshSchedulesDirect(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetAvailableCountries");
        await hub.Clients.All.DataRefresh("GetHeadendsToView");
        await hub.Clients.All.DataRefresh("GetSelectedStationIds");
        await hub.Clients.All.DataRefresh("GetStationChannelNames");
        await hub.Clients.All.DataRefresh("GetStationPreviews");
        await hub.Clients.All.DataRefresh("GetSubScribedHeadends");
        await hub.Clients.All.DataRefresh("GetSubscribedLineups");
    }

    public async Task RefreshSettings(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetSettings");
    }

    public async Task RefreshSMChannels(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedSMChannels");
        await hub.Clients.All.DataRefresh("GetSMChannelNameLogos");
        await hub.Clients.All.DataRefresh("GetSMChannelNames");
    }

    public async Task RefreshSMChannelStreamLinks(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetSMChannelStreams");
    }

    public async Task RefreshSMStreams(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedSMStreams");
    }

    public async Task RefreshSMTasks(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetSMTasks");
    }

    public async Task RefreshStatistics(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetChannelMetrics");
        await hub.Clients.All.DataRefresh("GetVideoInfos");
    }

    public async Task RefreshStreamGroups(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedStreamGroups");
        await hub.Clients.All.DataRefresh("GetStreamGroupProfiles");
        await hub.Clients.All.DataRefresh("GetStreamGroups");
    }

    public async Task RefreshStreamGroupSMChannelLinks(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetStreamGroupSMChannels");
    }

    public async Task RefreshVs(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetVs");
    }
}
