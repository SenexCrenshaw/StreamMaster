using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.Icons.Queries;
public record GetIconsSimpleQuery(IconFileParameters iconFileParameters) : IRequest<IEnumerable<IconFileDto>>;

internal class GetIconsSimpleQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetIconsSimpleQuery, IEnumerable<IconFileDto>>
{


    public GetIconsSimpleQueryHandler(ILogger<GetIconsSimpleQuery> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }

    public Task<IEnumerable<IconFileDto>> Handle(GetIconsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = MemoryCache.GetIcons(Mapper).Skip(request.iconFileParameters.First).ToList();
        List<IconFileDto> ficons = icons.Take(request.iconFileParameters.Count).ToList();
        IEnumerable<IconFileDto> ret = Mapper.Map<IEnumerable<IconFileDto>>(ficons);

        return Task.FromResult(ret);
    }
}