using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.M3UFiles.Queries;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IM3UFileHub
{
    public async Task CreateM3UFile(CreateM3UFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteM3UFile(DeleteM3UFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ProcessM3UFile(ProcessM3UFileRequest request)
    {
        await taskQueue.ProcessM3UFile(request.Id).ConfigureAwait(false);
        //await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshM3UFile(RefreshM3UFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForM3UFiles()
    {
        await mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateM3UFile(UpdateM3UFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters m3uFileParameters)
    {
        return await mediator.Send(new GetPagedM3UFiles(m3uFileParameters)).ConfigureAwait(false);
    }

    public async Task<M3UFileDto?> GetM3UFile(int id)
    {
        return await mediator.Send(new GetM3UFileByIdQuery(id)).ConfigureAwait(false);

    }

    public async Task<List<string>> GetM3UFileNames()
    {
        return await mediator.Send(new GetM3UFileNamesQuery()).ConfigureAwait(false);

    }
}