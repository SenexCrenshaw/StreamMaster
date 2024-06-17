using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.Services;
public partial class DataRefreshService : IDataRefreshServicePartial
{
    public async Task IsSystemReady()
    {

        await hub.Clients.All.IsSystemReady(BuildInfo.IsSystemReady);

    }

    public async Task TaskIsRunning()
    {

        await hub.Clients.All.TaskIsRunning(BuildInfo.IsTaskRunning);

    }

    public async Task Refresh(string command)
    {
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.DataRefresh(command);

    }

    public async Task RefreshAllEPG()
    {
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }
        await RefreshEPGFiles();
        await RefreshEPG();
        await RefreshSchedulesDirect();
    }

    public async Task RefreshAllM3U()
    {
        if (!BuildInfo.IsSystemReady)
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
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
        await RefreshStreamGroupSMChannelLinks();
        await RefreshChannelGroups();

    }

    public async Task ClearByTag(string Entity, string Tag)
    {
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.ClearByTag(new ClearByTag(Entity, Tag));
    }

    public async Task SetField(List<FieldData> fieldData)
    {
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.SetField(fieldData);
    }

    public async Task SendSMTasks(List<SMTask> smTasks)
    {
        //if (!BuildInfo.IsSystemReady)
        //{
        //    return;
        //}

        await hub.Clients.All.SendSMTasks(smTasks);
    }

    public async Task SendMessage(SMMessage smMessage)
    {
        if (!BuildInfo.IsSystemReady)
        {
            return;
        }

        await hub.Clients.All.SendMessage(smMessage);
    }

}
