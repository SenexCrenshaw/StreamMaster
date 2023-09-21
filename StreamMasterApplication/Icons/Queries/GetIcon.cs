using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcon(int Id) : IRequest<IconFileDto?>;

internal class GetIconQueryHandler : IRequestHandler<GetIcon, IconFileDto?>
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public GetIconQueryHandler(
            ISender sender,
             IMapper mapper
        )
    {
        _sender = sender;
        _mapper = mapper;
    }

    public async Task<IconFileDto?> Handle(GetIcon request, CancellationToken cancellationToken)
    {
        SettingDto setting = await _sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);

        IconFileParameters iconFileParameters = new();
        PagedResponse<IconFileDto> icons = await _sender.Send(new GetPagedIcons(iconFileParameters), cancellationToken).ConfigureAwait(false);

        IconFileDto? icon = icons.Data.FirstOrDefault(a => a.Id == request.Id);

        if (icon == null)
        {
            return null;
        }

        return _mapper.Map<IconFileDto>(icon);
    }
}