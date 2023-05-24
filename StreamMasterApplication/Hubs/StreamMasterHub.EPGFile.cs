using StreamMasterApplication.EPGFiles;
using StreamMasterApplication.EPGFiles.Commands;
using StreamMasterApplication.EPGFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IEPGFileHub
{
    public async Task<EPGFilesDto?> AddEPGFile(AddEPGFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> ChangeEPGFileName(ChangeEPGFileNameRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<int?> DeleteEPGFile(DeleteEPGFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> GetEPGFile(int id)
    {
        return await _mediator.Send(new GetEPGFile(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<EPGFilesDto>> GetEPGFiles()
    {
        return await _mediator.Send(new GetEPGFiles()).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> ProcessEPGFile(ProcessEPGFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> RefreshEPGFile(RefreshEPGFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> ScanDirectoryForEPGFiles()
    {
        return await _mediator.Send(new ScanDirectoryForEPGFilesRequest()).ConfigureAwait(false);
    }

    public async Task<EPGFilesDto?> UpdateEPGFile(UpdateEPGFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
