using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteChannelGroupRequest(int ChannelGroupId) : IRequest<APIResponse> { }

public class DeleteChannelGroupRequestHandler(IRepositoryWrapper Repository, IPublisher Publisher, IMemoryCache MemoryCache)
    : IRequestHandler<DeleteChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {

        (int? ChannelGroupId, IEnumerable<VideoStreamDto> VideoStreams) = await Repository.ChannelGroup.DeleteChannelGroupById(request.ChannelGroupId).ConfigureAwait(false); ;
        _ = await Repository.SaveAsync().ConfigureAwait(false);
        if (ChannelGroupId != null)
        {
            MemoryCache.RemoveChannelGroupStreamCount((int)ChannelGroupId);
            foreach (VideoStreamDto item in VideoStreams)
            {
                item.User_Tvg_group = "";
            }
            await Publisher.Publish(new DeleteChannelGroupEvent((int)ChannelGroupId, VideoStreams), cancellationToken);

            return APIResponse.Success;
        }

        return APIResponse.NotFound;
    }
}
