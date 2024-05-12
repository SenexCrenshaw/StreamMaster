using Microsoft.AspNetCore.SignalR;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Hubs;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services;

public partial class DataRefreshService(IHubContext<StreamMasterHub, IStreamMasterHub> hub) : IDataRefreshService, IDataRefreshServicePartial
{

    public async Task RefreshAll()
    {

        await RefreshChannelGroups(true);
        await RefreshEPG(true);
        await RefreshEPGFiles(true);
        await RefreshIcons(true);
        await RefreshM3UFiles(true);
        await RefreshSchedulesDirect(true);
        await RefreshSettings(true);
        await RefreshSMChannels(true);
        await RefreshSMChannelStreamLinks(true);
        await RefreshSMStreams(true);
        await RefreshStreamGroups(true);
        await RefreshStreamGroupSMChannelLinks(true);
    }

    public async Task RefreshChannelGroups(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetChannelGroups");
        await hub.Clients.All.DataRefresh("GetPagedChannelGroups");
    }

    public async Task RefreshEPG(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetEPGColors");
    }

    public async Task RefreshEPGFiles(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetEPGFilePreviewById");
        await hub.Clients.All.DataRefresh("GetEPGFiles");
        await hub.Clients.All.DataRefresh("GetEPGNextEPGNumber");
        await hub.Clients.All.DataRefresh("GetM3UFiles");
        await hub.Clients.All.DataRefresh("GetPagedEPGFiles");
    }

    public async Task RefreshIcons(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetIcons");
    }

    public async Task RefreshM3UFiles(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetM3UFileNames");
        await hub.Clients.All.DataRefresh("GetPagedM3UFiles");
    }

    public async Task RefreshSchedulesDirect(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetStationChannelNames");
    }

    public async Task RefreshSettings(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetIsSystemReady");
        await hub.Clients.All.DataRefresh("GetSettings");
        await hub.Clients.All.DataRefresh("GetSystemStatus");
    }

    public async Task RefreshSMChannels(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedSMChannels");
        await hub.Clients.All.DataRefresh("GetSMChannel");
        await hub.Clients.All.DataRefresh("GetSMChannelNames");
    }

    public async Task RefreshSMChannelStreamLinks(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetSMChannelStreams");
    }

    public async Task RefreshSMStreams(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedSMStreams");
    }

    public async Task RefreshStreamGroups(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetPagedStreamGroups");
        await hub.Clients.All.DataRefresh("GetStreamGroups");
    }

    public async Task RefreshStreamGroupSMChannelLinks(bool alwaysRun = false)
    {

        if (!alwaysRun && !BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh("GetStreamGroupSMChannels");
    }
}
