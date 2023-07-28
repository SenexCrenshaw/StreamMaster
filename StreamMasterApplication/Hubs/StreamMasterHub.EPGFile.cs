using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task AddEPGFile(AddEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeEPGFileName(ChangeEPGFileNameRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteEPGFile(DeleteEPGFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> GetEPGFile(int id)
    {
        return await _mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<EPGFilesDto>> GetEPGFiles()
    {
        return await _mediator.Send(new GetEPGFiles()).ConfigureAwait(false);
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
