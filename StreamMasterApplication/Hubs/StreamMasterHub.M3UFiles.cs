using StreamMasterApplication.M3UFiles;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.M3UFiles.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IM3UFileHub
{
    public async Task<M3UFilesDto?> AddM3UFile(AddM3UFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<M3UFilesDto?> ChangeM3UFileName(ChangeM3UFileNameRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<int?> DeleteM3UFile(DeleteM3UFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<M3UFilesDto?> GetM3UFile(int id)
    {
        return await _mediator.Send(new GetM3UFile(id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<M3UFilesDto>> GetM3UFiles()
    {
        return await _mediator.Send(new GetM3UFiles()).ConfigureAwait(false);
    }

    public async Task<M3UFilesDto?> ProcessM3UFile(ProcessM3UFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
        //if (data == null)
        //{
        //    return null;
        //}

        //await Clients.All.M3UFilesDtoUpdate(data).ConfigureAwait(false);
        //return data;
    }

    public async Task<M3UFilesDto?> RefreshM3UFile(RefreshM3UFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> ScanDirectoryForM3UFiles()
    {
        return await _mediator.Send(new ScanDirectoryForM3UFilesRequest()).ConfigureAwait(false);
    }

    public async Task<M3UFilesDto?> UpdateM3UFile(UpdateM3UFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
