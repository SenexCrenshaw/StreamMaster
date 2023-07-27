using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class SetVideoStreamsLogoToEPGRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class SetVideoStreamsLogoToEPGHandler : IRequestHandler<SetVideoStreamsLogoToEPGRequest, List<VideoStreamDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public SetVideoStreamsLogoToEPGHandler(
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

    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamsLogoToEPGRequest request, CancellationToken cancellationToken)
    {
        var results = new List<VideoStreamDto>();
        var videoStreams = await _context.VideoStreams.Where(a => request.Ids.Contains(a.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var videoStream in videoStreams)
        {
            var channelLogo = _memoryCache.GetEPGChannelByTvgId(videoStream.User_Tvg_ID);

            if (channelLogo != null)
            {
                videoStream.User_Tvg_logo = channelLogo;
                results.Add(_mapper.Map<VideoStreamDto>(videoStream));
            }
        }

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        if (results.Count > 0)
        {
            await _publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
