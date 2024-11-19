namespace StreamMaster.Application.SMChannelChannelLinks.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMChannelChannelsRequest(int SMChannelId) : IRequest<DataResponse<List<SMChannelDto>>>;

internal class GetSMChannelChannelssRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetSMChannelChannelsRequest, DataResponse<List<SMChannelDto>>>
{
    public async Task<DataResponse<List<SMChannelDto>>> Handle(GetSMChannelChannelsRequest request, CancellationToken cancellationToken)
    {
        SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (channel == null)
        {
            return DataResponse<List<SMChannelDto>>.ErrorWithMessage("Failed to retreieve");

        }

        List<SMChannelChannelLink> links = await Repository.SMChannelChannelLink.GetQuery(true).Where(a => a.ParentSMChannelId == request.SMChannelId).ToListAsync();
        List<SMChannelDto> ret = [];
        foreach (SMChannel smChannel in channel.SMChannels.Select(a => a.SMChannel))
        {
            SMChannelChannelLink? link = links.Find(a => a.SMChannelId == smChannel.Id);

            if (link != null)
            {
                SMChannelDto dto = mapper.Map<SMChannelDto>(smChannel);
                dto.Rank = link.Rank;
                ret.Add(dto);
            }
        }

        return await Task.FromResult(DataResponse<List<SMChannelDto>>.Success([.. ret.OrderBy(a => a.Rank)]));
    }


}
