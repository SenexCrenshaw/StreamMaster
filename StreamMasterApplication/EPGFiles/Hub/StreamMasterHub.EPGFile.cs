using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task CreateEPGFile(CreateEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteEPGFile(DeleteEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFileDto?> GetEPGFile(int id)
    {
        return await mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters parameters)
    {
        return await mediator.Send(new GetPagedEPGFiles(parameters)).ConfigureAwait(false);
    }

    public async Task ProcessEPGFile(ProcessEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshEPGFile(RefreshEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForEPGFiles()
    {
        await mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateEPGFile(UpdateEPGFileRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }
}