using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Icons.Queries;
public record GetIconsSimpleQuery(IconFileParameters iconFileParameters) : IRequest<IEnumerable<IconFileDto>>;

internal class GetIconsSimpleQueryHandler(ILogger<GetIconsSimpleQuery> logger, IIconService iconService, IMapper Mapper)
    : IRequestHandler<GetIconsSimpleQuery, IEnumerable<IconFileDto>>
{
    public Task<IEnumerable<IconFileDto>> Handle(GetIconsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = iconService.GetIcons().Skip(request.iconFileParameters.First).ToList();
        List<IconFileDto> ficons = icons.Take(request.iconFileParameters.Count).ToList();
        IEnumerable<IconFileDto> ret = Mapper.Map<IEnumerable<IconFileDto>>(ficons);

        return Task.FromResult(ret);
    }
}