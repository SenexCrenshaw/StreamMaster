namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;


public record MoveSMChannelsToStreamGroupByM3UFileIdRequest(M3UFile M3UFile, int OldStreamGroupId) : IRequest<APIResponse>;

internal class MoveSMChannelsToStreamGroupRequestHandler(IRepositoryWrapper Repository, IRepositoryContext repositoryContext, IDataRefreshService dataRefreshService) : IRequestHandler<MoveSMChannelsToStreamGroupByM3UFileIdRequest, APIResponse>
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

        var sg = await Repository.StreamGroup.GetStreamGroupByName(request.M3UFile.DefaultStreamGroupName);
        if (sg is null)
        {
            return APIResponse.ErrorWithMessage("SG not found");
        }

        var channelsIds = await Repository.SMChannel.GetQuery().Where(a => a.M3UFileId == request.M3UFile.Id).Select(a => a.Id).ToListAsync();
        if (channelsIds.Count == 0)
        {
            return APIResponse.Success;
        }

        var links = Repository.StreamGroupSMChannelLink.GetQuery().Where(a => channelsIds.Contains(a.SMChannelId) && a.StreamGroupId == request.OldStreamGroupId);
        if (!links.Any())
        {
            return APIResponse.Success;
        }

        await Repository.StreamGroupSMChannelLink.RemoveSMChannelsFromStreamGroup(request.OldStreamGroupId, channelsIds);
        await Repository.SaveAsync();
        await Repository.StreamGroupSMChannelLink.AddSMChannelsToStreamGroup(sg.Id, channelsIds);

        return APIResponse.Success;
    }
}
