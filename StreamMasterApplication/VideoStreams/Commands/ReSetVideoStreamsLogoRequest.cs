using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class ReSetVideoStreamsLogoRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class ReSetVideoStreamsLogoHandler : IRequestHandler<ReSetVideoStreamsLogoRequest, List<VideoStreamDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public ReSetVideoStreamsLogoHandler(
        IMapper mapper,
          ISender sender,
          IMemoryCache memoryCache,
         IPublisher publisher,
        IAppDbContext context
        )
    {
        _memoryCache = memoryCache;
        _sender = sender;
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<VideoStreamDto>> Handle(ReSetVideoStreamsLogoRequest request, CancellationToken cancellationToken)
    {
   
        var videoStreams = await _context.VideoStreams.Where(a => request.Ids.Contains(a.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var videoStream in videoStreams)
        {
            videoStream.User_Tvg_logo = videoStream.Tvg_logo;
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var ret = _mapper.Map<List<VideoStreamDto>>(videoStreams);

        await _publisher.Publish(new UpdateVideoStreamsEvent(ret), cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
