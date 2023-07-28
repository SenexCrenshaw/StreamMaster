using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IM3UFileHub
{
    public async Task AddM3UFile(AddM3UFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteM3UFile(DeleteM3UFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<M3UFilesDto?> GetM3UFile(int id)
    {
        return await _mediator.Send(new GetM3UFile(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<M3UFilesDto>> GetM3UFiles()
    {
        return await _mediator.Send(new GetM3UFiles()).ConfigureAwait(false);
    }

    public async Task ProcessM3UFile(ProcessM3UFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task RefreshM3UFile(RefreshM3UFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task ScanDirectoryForM3UFiles()
    {
        await _mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
    }

    public async Task UpdateM3UFile(UpdateM3UFileRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }
}
