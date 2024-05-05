using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Hubs;

namespace StreamMaster.Infrastructure.Services;

public partial class DataRefreshService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, ISender sender) : IDataRefreshService, IDataRefreshServicePartial
{

    public async Task RefreshAll()
    {
        await RefreshChannelGroups();
        await RefreshEPG();
        await RefreshEPGFiles();
        await RefreshIcons();
        await RefreshM3UFiles();
        await RefreshSchedulesDirect();
        await RefreshSettings();
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
        await RefreshSMStreams();
        await RefreshStreamGroups();
        await RefreshStreamGroupSMChannelLinks();
    }

    public async Task RefreshChannelGroups()
    {
        await hub.Clients.All.DataRefresh("GetChannelGroups");
        await hub.Clients.All.DataRefresh("GetPagedChannelGroups");
    }

    public async Task RefreshEPG()
    {
        await hub.Clients.All.DataRefresh("GetEPGColors");
    }

    public async Task RefreshEPGFiles()
    {
        await hub.Clients.All.DataRefresh("GetEPGFilePreviewById");
        await hub.Clients.All.DataRefresh("GetEPGFiles");
        await hub.Clients.All.DataRefresh("GetEPGNextEPGNumber");
        await hub.Clients.All.DataRefresh("GetPagedEPGFiles");
    }

    public async Task RefreshIcons()
    {
        await hub.Clients.All.DataRefresh("GetIcons");
    }

    public async Task RefreshM3UFiles()
    {
        await hub.Clients.All.DataRefresh("GetM3UFileNames");
        await hub.Clients.All.DataRefresh("GetPagedM3UFiles");
    }

    public async Task RefreshSchedulesDirect()
    {
        await hub.Clients.All.DataRefresh("GetStationChannelNames");
    }

    public async Task RefreshSettings()
    {
        await hub.Clients.All.DataRefresh("GetIsSystemReady");
        await hub.Clients.All.DataRefresh("GetSettings");
        await hub.Clients.All.DataRefresh("GetSystemStatus");
    }

    public async Task RefreshSMChannels()
    {
        await hub.Clients.All.DataRefresh("GetPagedSMChannels");
        await hub.Clients.All.DataRefresh("GetSMChannelNames");
    }

    public async Task RefreshSMChannelStreamLinks()
    {
        await hub.Clients.All.DataRefresh("GetSMChannelStreams");
    }

    public async Task RefreshSMStreams()
    {
        await hub.Clients.All.DataRefresh("GetPagedSMStreams");
    }

    public async Task RefreshStreamGroups()
    {
        await hub.Clients.All.DataRefresh("GetPagedStreamGroups");
        await hub.Clients.All.DataRefresh("GetStreamGroups");
    }

    public async Task RefreshStreamGroupSMChannelLinks()
    {
        await hub.Clients.All.DataRefresh("GetStreamGroupSMChannels");
    }
}
