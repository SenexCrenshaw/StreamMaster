using StreamMasterApplication.Icons;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IIconHub
{

    public async Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IconFileDto?> GetIcon(int Id)
    {
        return await _mediator.Send(new GetIcon(Id)).ConfigureAwait(false);
    }

    public async Task<IconFileDto?> GetIconFromSource(string source)
    {
        return await _mediator.Send(new GetIconFromSource(source)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<IconFileDto>> GetIcons(IconFileParameters iconFileParameters)
    {
        PagedResponse<IconFileDto> data = await _mediator.Send(new GetIcons(iconFileParameters)).ConfigureAwait(false);
        return data;
    }

    public async Task<IEnumerable<IconSimpleDto>> GetIconsSimpleQuery(IconFileParameters iconFileParameters)
    {
        IEnumerable<IconSimpleDto> data = await _mediator.Send(new GetIconsSimpleQuery(iconFileParameters)).ConfigureAwait(false);
        return data;
    }
}