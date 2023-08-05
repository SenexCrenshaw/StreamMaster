using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.VideoStreams.Commands;

public class ReSetVideoStreamsLogoRequest : IRequest<List<VideoStreamDto>>
{
    public List<string> Ids { get; set; } = new List<string>();
}

public class ReSetVideoStreamsLogoHandler : BaseMediatorRequestHandler, IRequestHandler<ReSetVideoStreamsLogoRequest, List<VideoStreamDto>>
{

    public ReSetVideoStreamsLogoHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<List<VideoStreamDto>> Handle(ReSetVideoStreamsLogoRequest request, CancellationToken cancellationToken)
    {

        var videoStreamsRepo = await Repository.VideoStream.GetAllVideoStreamsAsync().ConfigureAwait(false);
        var videoStreams = videoStreamsRepo.Where(a => request.Ids.Contains(a.Id));


        foreach (var videoStream in videoStreams)
        {
            videoStream.User_Tvg_logo = videoStream.Tvg_logo;
            Repository.VideoStream.Update(videoStream);
        }

        await Repository.SaveAsync().ConfigureAwait(false);

        var ret = Mapper.Map<List<VideoStreamDto>>(videoStreams);
        await Publisher.Publish(new UpdateVideoStreamsEvent(ret), cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
