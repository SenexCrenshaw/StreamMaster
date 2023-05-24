using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroups : IRequest<IEnumerable<ChannelGroupDto>>;

internal class GetChannelGroupsHandler : IRequestHandler<GetChannelGroups, IEnumerable<ChannelGroupDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetChannelGroupsHandler(
         IAppDbContext context,
         IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ChannelGroupDto>> Handle(GetChannelGroups request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> ret = await _context.ChannelGroups
            .AsNoTracking()
            .ProjectTo<ChannelGroupDto>(_mapper.ConfigurationProvider)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
