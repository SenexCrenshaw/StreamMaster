using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.CommandsOrig;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub
{

    public async Task ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForM3UFiles()
    {
        await Sender.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
    }

}