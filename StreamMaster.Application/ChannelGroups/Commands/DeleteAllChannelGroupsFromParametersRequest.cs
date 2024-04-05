using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteAllChannelGroupsFromParametersRequest(QueryStringParameters Parameters) : IRequest<DefaultAPIResponse> { }

public class DeleteAllChannelGroupsFromParametersRequestHandler(IRepositoryWrapper Repository, IPublisher Publisher, IMemoryCache memoryCache)
    : IRequestHandler<DeleteAllChannelGroupsFromParametersRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteAllChannelGroupsFromParametersRequest request, CancellationToken cancellationToken)
    {
        (IEnumerable<int> ChannelGroupIds, IEnumerable<VideoStreamDto> VideoStreams) = await Repository.ChannelGroup.DeleteAllChannelGroupsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        await Repository.VideoStream.UpdateVideoStreamsChannelGroupNames(VideoStreams.Select(a => a.Id), "").ConfigureAwait(false);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        if (ChannelGroupIds.Any())
        {

            foreach (int id in ChannelGroupIds)
            {
                memoryCache.RemoveChannelGroupStreamCount(id);
            }


            foreach (VideoStreamDto item in VideoStreams)
            {
                item.User_Tvg_group = "";
            }
            await Publisher.Publish(new DeleteChannelGroupsEvent(ChannelGroupIds, VideoStreams), cancellationToken);

            return DefaultAPIResponse.Success;
        }

        return DefaultAPIResponse.NotFound;
    }
}
