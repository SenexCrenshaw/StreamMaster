using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelIdFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<int?>;

internal class GetChannelIdFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelIdFromVideoStream, int?>
{

    public GetChannelIdFromVideoStreamHandler(ILogger<GetChannelIdFromVideoStream> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<int?> Handle(GetChannelIdFromVideoStream request, CancellationToken cancellationToken)
    {
        int? id = await Repository.ChannelGroup.GetChannelGroupIdFromVideoStream(request.VideoStreamDto.User_Tvg_group).ConfigureAwait(false);
        return id;
    }
}