using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroup(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler : IRequestHandler<GetChannelGroup, ChannelGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetChannelGroupHandler(
         IAppDbContext context,
         IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ChannelGroupDto?> Handle(GetChannelGroup request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await _context.ChannelGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        ChannelGroupDto ret = _mapper.Map<ChannelGroupDto>(channelGroup);

        return ret;
    }
}
