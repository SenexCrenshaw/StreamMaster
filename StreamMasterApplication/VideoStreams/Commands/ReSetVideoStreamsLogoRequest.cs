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

        IQueryable<VideoStream> videoStreams = Repository.VideoStream.GetAllVideoStreams().Where(a => request.Ids.Contains(a.Id));

        foreach (VideoStream? videoStream in videoStreams)
        {
            videoStream.User_Tvg_logo = videoStream.Tvg_logo;
            Repository.VideoStream.Update(videoStream);
        }

        await Repository.SaveAsync().ConfigureAwait(false);


        await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        List<VideoStreamDto> ret = Mapper.Map<List<VideoStreamDto>>(videoStreams);
        return ret;
    }
}
