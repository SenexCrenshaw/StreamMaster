using StreamMasterApplication.Icons;
using StreamMasterApplication.Icons.Commands;
using StreamMasterApplication.Icons.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IIconHub
{

    public async Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<IconFileDto?> GetIcon(int Id)
    {
        return await mediator.Send(new GetIcon(Id)).ConfigureAwait(false);
    }

    public async Task<IconFileDto?> GetIconFromSource(GetIconFromSourceRequest request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }


    public async Task<PagedResponse<IconFileDto>> GetPagedIcons(IconFileParameters iconFileParameters)
    {
        PagedResponse<IconFileDto> data = await mediator.Send(new GetPagedIcons(iconFileParameters)).ConfigureAwait(false);
        return data;
    }

    public async Task<IEnumerable<IconFileDto>> GetIconsSimpleQuery(IconFileParameters iconFileParameters)
    {
        IEnumerable<IconFileDto> data = await mediator.Send(new GetIconsSimpleQuery(iconFileParameters)).ConfigureAwait(false);
        return data;
    }
}