using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class SetVideoStreamSetEPGsFromNameRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class SetVideoStreamSetEPGsFromNameRequestHandler : IRequestHandler<SetVideoStreamSetEPGsFromNameRequest, List<VideoStreamDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublisher _publisher;
    private readonly ISender _sender;

    public SetVideoStreamSetEPGsFromNameRequestHandler(
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

    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamSetEPGsFromNameRequest request, CancellationToken cancellationToken)
    {
        var results = new List<VideoStreamDto>();
        var videoStreams = await _context.VideoStreams.Where(a => request.Ids.Contains(a.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var videoStream in videoStreams)
        {

            var test = _memoryCache.GetEPGNameTvgName(videoStream.User_Tvg_name);
            if (test is not null)
            {
                videoStream.User_Tvg_ID = test;
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
