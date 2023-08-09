using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class SetVideoStreamsLogoToEPGRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class SetVideoStreamsLogoToEPGHandler : BaseMemoryRequestHandler, IRequestHandler<SetVideoStreamsLogoToEPGRequest, List<VideoStreamDto>>
{

    public SetVideoStreamsLogoToEPGHandler(ILogger<DeleteM3UFileHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamsLogoToEPGRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = new();
        IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetAllVideoStreams();

        foreach (VideoStream videoStream in videoStreams)
        {
            string? channelLogo = MemoryCache.GetEPGChannelByTvgId(videoStream.User_Tvg_ID);

            if (channelLogo != null)
            {
                videoStream.User_Tvg_logo = channelLogo;
                Repository.VideoStream.Update(videoStream);
                results.Add(Mapper.Map<VideoStreamDto>(videoStream));
            }
        }

        await Repository.SaveAsync().ConfigureAwait(false);
        if (results.Count > 0)
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }

        return results;
    }
}
