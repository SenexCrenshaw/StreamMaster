namespace StreamMaster.Application.ChannelGroups.QueriesOld;

public record GetChannelGroupNameFromVideoStream(VideoStreamDto VideoStreamDto) : IRequest<string?>;

internal class GetChannelGroupNameFromVideoStreamHandler(ILogger<GetChannelGroupNameFromVideoStream> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetChannelGroupNameFromVideoStream, string?>
{
    public async Task<string?> Handle(GetChannelGroupNameFromVideoStream request, CancellationToken cancellationToken)
    {

        string? name = await Repository.ChannelGroup.GetChannelGroupNameFromVideoStream(request.VideoStreamDto.User_Tvg_group).ConfigureAwait(false);
        return name;
    }
}