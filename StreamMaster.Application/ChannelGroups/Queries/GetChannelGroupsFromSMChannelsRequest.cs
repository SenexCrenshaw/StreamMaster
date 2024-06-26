namespace StreamMaster.Application.ChannelGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelGroupsFromSMChannelsRequest() : IRequest<DataResponse<List<ChannelGroupDto>>>;

internal class GetChannelGroupsFromSMChannelsRequestHandler(IRepositoryWrapper Repository, IMapper mapper, IMemoryCache MemoryCache)
    : IRequestHandler<GetChannelGroupsFromSMChannelsRequest, DataResponse<List<ChannelGroupDto>>>
{
    public async Task<DataResponse<List<ChannelGroupDto>>> Handle(GetChannelGroupsFromSMChannelsRequest request, CancellationToken cancellationToken)
    {
        List<string> cgNames = await Repository.SMChannel.GetQuery().Select(a => a.Group).Distinct().ToListAsync(cancellationToken: cancellationToken);
        List<ChannelGroup> channelGroups = await Repository.ChannelGroup.GetQuery().Where(a => cgNames.Contains(a.Name)).ToListAsync(cancellationToken: cancellationToken);
        List<ChannelGroupDto> channelGroupDtos = mapper.Map<List<ChannelGroupDto>>(channelGroups);
        return DataResponse<List<ChannelGroupDto>>.Success(channelGroupDtos);
    }
}