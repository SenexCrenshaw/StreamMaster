using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler : IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetStreamGroupHandler(
         IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0) return new StreamGroupDto { Id = 0, Name = "All" };

        StreamGroup? streamGroup = await _context.StreamGroups
            .Include(a => a.VideoStreams)
            .Include(a => a.ChannelGroups)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);


        if ( streamGroup == null)
            return null;

        var ret = _mapper.Map<StreamGroupDto>(streamGroup);

        var existingIds= streamGroup.VideoStreams.Select(a => a.Id).ToList();

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
            ret.VideoStreams.AddRange(streams);
        }

        return ret;
    }
}
