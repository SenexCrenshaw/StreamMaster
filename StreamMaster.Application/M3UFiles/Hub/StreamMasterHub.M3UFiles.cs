using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IM3UFileHub
{
    public async Task CreateM3UFile(CreateM3UFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteM3UFile(DeleteM3UFileRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task ProcessM3UFile(ProcessM3UFileRequest request)
    {
        await taskQueue.ProcessM3UFile(request).ConfigureAwait(false);
    }

    public async Task RefreshM3UFile(RefreshM3UFileRequest request)
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

    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters m3uFileParameters)
    {
        return await Sender.Send(new GetPagedM3UFiles(m3uFileParameters)).ConfigureAwait(false);
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