﻿namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNameFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<string?>;

internal class GetChannelGroupNameFromVideoStreamHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupNameFromVideoStream, string?>
{

    public GetChannelGroupNameFromVideoStreamHandler(ILogger<GetChannelGroupNameFromVideoStream> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<string?> Handle(GetChannelGroupNameFromVideoStream request, CancellationToken cancellationToken)
    {

        string? name = await Repository.ChannelGroup.GetChannelGroupNameFromVideoStream(request.VideoStreamDto.User_Tvg_group).ConfigureAwait(false);
        return name;
    }
}