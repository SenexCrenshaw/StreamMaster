using StreamMasterApplication.Icons;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IIconHub
{
    public async Task<IconFileDto?> AddIconFile(AddIconFileRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IconFileDto?> GetIcon(int Id)
    {
        return await _mediator.Send(new GetIcon(Id)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<IconFileDto>> GetIcons()
    {
        IEnumerable<IconFileDto> data = await _mediator.Send(new GetIcons()).ConfigureAwait(false);
        return data.ToList();
    }
}
