using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons.Queries;
public record GetIconsSimpleQuery(IconFileParameters iconFileParameters) : IRequest<IEnumerable<IconFileDto>>;

internal class GetIconsSimpleQueryHandler : BaseMemoryRequestHandler, IRequestHandler<GetIconsSimpleQuery, IEnumerable<IconFileDto>>
{

    public GetIconsSimpleQueryHandler(ILogger<GetIconsSimpleQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public Task<IEnumerable<IconFileDto>> Handle(GetIconsSimpleQuery request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = MemoryCache.GetIcons(Mapper).Skip(request.iconFileParameters.First).ToList();
        List<IconFileDto> ficons = icons.Take(request.iconFileParameters.Count).ToList();
        IEnumerable<IconFileDto> ret = Mapper.Map<IEnumerable<IconFileDto>>(ficons);

        return Task.FromResult(ret);
    }
}