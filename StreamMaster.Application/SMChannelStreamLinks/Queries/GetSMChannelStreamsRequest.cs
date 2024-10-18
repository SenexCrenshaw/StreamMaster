namespace StreamMaster.Application.SMChannelStreamLinks.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSMChannelStreamsRequest(int SMChannelId) : IRequest<DataResponse<List<SMStreamDto>>>;

internal class GetSMChannelStreamssRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetSMChannelStreamsRequest, DataResponse<List<SMStreamDto>>>
{
    public async Task<DataResponse<List<SMStreamDto>>> Handle(GetSMChannelStreamsRequest request, CancellationToken cancellationToken)
    {
        SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (channel == null)
        {
            return DataResponse<List<SMStreamDto>>.ErrorWithMessage("Failed to retreieve");

        }

        List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId)];
        List<SMStreamDto> ret = [];
        foreach (SMStream? stream in channel.SMStreams.Select(a => a.SMStream))
        {
            SMChannelStreamLink? link = links.Find(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                SMStreamDto dto = mapper.Map<SMStreamDto>(stream);
                dto.Rank = link.Rank;
                ret.Add(dto);
            }
        }

        return await Task.FromResult(DataResponse<List<SMStreamDto>>.Success([.. ret.OrderBy(a => a.Rank)]));
    }


}
