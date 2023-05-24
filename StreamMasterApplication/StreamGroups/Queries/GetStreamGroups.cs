using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsHandler : IRequestHandler<GetStreamGroups, IEnumerable<StreamGroupDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetStreamGroupsHandler(
        IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {
        var ret = await _context.StreamGroups
           .Include(a => a.VideoStreams)
           .AsNoTracking()
           .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Name)
           .ToListAsync(cancellationToken).ConfigureAwait(false);


        ret.Insert(0, new StreamGroupDto { Id = 0, Name = "All" });

        return ret;
    }
}
