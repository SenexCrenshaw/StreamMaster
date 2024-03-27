using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.CommandsOrig;
using StreamMaster.Application.M3UFiles.Queries;

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

    public async Task UpdateM3UFile(UpdateM3UFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<M3UFileDto?> GetM3UFile(int id)
    {
        return await Sender.Send(new GetM3UFileQuery(id)).ConfigureAwait(false);

    }

    public async Task<List<string>> GetM3UFileNames()
    {
        return await Sender.Send(new GetM3UFileNamesQuery()).ConfigureAwait(false);

    }
}