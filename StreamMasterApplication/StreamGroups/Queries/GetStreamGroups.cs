using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

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
           .Include(a => a.ChannelGroups)
           .AsNoTracking()
           .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Name)
           .ToListAsync(cancellationToken).ConfigureAwait(false);


        foreach (var streamGroup in ret)
        {
            var existingIds = streamGroup.VideoStreams.Select(a => a.Id).ToList();

            foreach (var channegroup in streamGroup.ChannelGroups)
            {
                var streams = _context.VideoStreams
                    .Where(a => !existingIds.Contains(a.Id) && a.User_Tvg_group == channegroup.Name)
                    .AsNoTracking()
                    .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                    .ToList();
                foreach (var stream in streams)
                {
                    stream.IsReadOnly = true;
                }
                streamGroup.VideoStreams.AddRange(streams);
            }
        }

        ret.Insert(0, new StreamGroupDto { Id = 0, Name = "All" });

        return ret;
    }
}
