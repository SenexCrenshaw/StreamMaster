using AutoMapper;

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
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        var ret = _mapper.Map<StreamGroupDto>(streamGroup);

        return streamGroup == null ? null : _mapper.Map<StreamGroupDto>(streamGroup);
    }
}
