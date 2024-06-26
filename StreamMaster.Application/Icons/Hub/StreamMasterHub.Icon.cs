using StreamMaster.Application.Icons.CommandsOld;
namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub
{

    public async Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

}