namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedStreamGroupsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedStreamGroupsRequest, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {

        var ret = request.Parameters.PageSize == 0
            ? Repository.StreamGroup.CreateEmptyPagedResponse()
            : await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);

        //if (ret.Count > 0)
        //{
        //    var profiles = Repository.StreamGroupProfile.GetStreamGroupProfiles();
        //    foreach (var sg in ret.Data)
        //    {
        //        sg.StreamGroupProfiles = profiles.Where(a => a.StreamGroupId == sg.Id).ToList();
        //    }

        //}
        foreach (var streamGroupDto in ret.Data)
        {
            if (streamGroupDto.Id == 1)
            {
                streamGroupDto.ChannelCount = Repository.SMChannel.Count();
                continue;
            }

            streamGroupDto.ChannelCount = Repository.StreamGroupSMChannelLink.GetQuery().Count(a => a.StreamGroupId == streamGroupDto.Id);
        }

        return ret;
    }
}