using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services;
public partial class DataRefreshService : IDataRefreshServicePartial
{

    public async Task RefreshAllEPG()
    {
        if (!BuildInfo.SetIsSystemReady)
        {
            return;
        }
        await RefreshEPGFiles();
        await RefreshEPG();
        await RefreshSchedulesDirect();
    }

    public async Task RefreshAllM3U()
    {
        if (!BuildInfo.SetIsSystemReady)
        {
            return;
        }
        await RefreshM3UFiles();
        await RefreshSMStreams();
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
        await RefreshChannelGroups();
    }

    public async Task RefreshAllSMChannels()
    {
        if (!BuildInfo.SetIsSystemReady)
        {
            return;
        }
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
        await RefreshStreamGroupSMChannelLinks();
    }

    public async Task ClearByTag(string Entity, string Tag)
    {
        if (!BuildInfo.SetIsSystemReady)
        {
            return;
        }

        await hub.Clients.All.ClearByTag(new ClearByTag(Entity, Tag));
    }

}
