using StreamMaster.Application.Services;

namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

public record MoveSMChannelsToStreamGroupByM3UFileIdRequest(M3UFile M3UFile, int OldStreamGroupId) : IRequest<APIResponse>;

internal class MoveSMChannelsToStreamGroupRequestHandler(IBackgroundTaskQueue taskQueue, IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<MoveSMChannelsToStreamGroupByM3UFileIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(MoveSMChannelsToStreamGroupByM3UFileIdRequest request, CancellationToken cancellationToken)
    {
        if (request.M3UFile is null)
        {
            return APIResponse.ErrorWithMessage("M3UFile not found");
        }

        if (string.IsNullOrEmpty(request.M3UFile.DefaultStreamGroupName))
        {
            return APIResponse.ErrorWithMessage($"Default StreamGroup {request.M3UFile.DefaultStreamGroupName} not found");
        }

        StreamGroupDto? sg = await Repository.StreamGroup.GetStreamGroupByName(request.M3UFile.DefaultStreamGroupName);
        if (sg is null)
        {
            return APIResponse.ErrorWithMessage("SG not found");
        }

        List<int> channelsIds = await Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFile.Id).Select(a => a.Id).ToListAsync(cancellationToken: cancellationToken);
        if (channelsIds.Count == 0)
        {
            return APIResponse.Success;
        }

        IQueryable<StreamGroupSMChannelLink> links = Repository.StreamGroupSMChannelLink.GetQuery().Where(a => channelsIds.Contains(a.SMChannelId) && a.StreamGroupId == request.OldStreamGroupId);
        if (!links.Any())
        {
            return APIResponse.Success;
        }

        _ = await Repository.StreamGroupSMChannelLink.RemoveSMChannelsFromStreamGroup(request.OldStreamGroupId, channelsIds);
        _ = await Repository.SaveAsync();
        await Repository.StreamGroupSMChannelLink.AddSMChannelsToStreamGroupAsync(sg.Id, channelsIds);
        await dataRefreshService.RefreshStreamGroups();
        await taskQueue.CreateSTRMFiles(cancellationToken);
        return APIResponse.Success;
    }
}
