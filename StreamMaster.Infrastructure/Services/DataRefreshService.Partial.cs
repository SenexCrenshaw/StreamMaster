using StreamMaster.Application.Common.Interfaces;
namespace StreamMaster.Infrastructure.Services;
public partial class DataRefreshService : IDataRefreshServicePartial
{

    public async Task SetIsReady(bool IsReady)
    {
        //await sender.Send(new SetIsSystemReadyRequest(false)).ConfigureAwait(false);
        //FieldData fieldData = new("GetIsSystemReady", "");
    }
    public async Task RefreshAllEPG()
    {
        await RefreshEPGFiles();
        await RefreshEPG();
    }

    public async Task RefreshAllM3U()
    {
        await RefreshM3UFiles();
        await RefreshSMStreams();
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
        await RefreshChannelGroups();
    }

    public async Task RefreshAllSMChannels()
    {
        await RefreshSMChannels();
        await RefreshSMChannelStreamLinks();
    }

}
