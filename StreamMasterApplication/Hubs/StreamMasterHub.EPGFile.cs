using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task CreateEPGFile(CreateEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteEPGFile(DeleteEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFileDto?> GetEPGFile(int id)
    {
        return await _mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<EPGFileDto>> GetEPGFiles(EPGFileParameters parameters)
    {
        return await _mediator.Send(new GetEPGFiles(parameters)).ConfigureAwait(false);
    }

    public async Task ProcessEPGFile(ProcessEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshEPGFile(RefreshEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForEPGFiles()
    {
        await _mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateEPGFile(UpdateEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }
}