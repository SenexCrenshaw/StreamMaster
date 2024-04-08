using StreamMaster.Domain.Exceptions;

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
            throw new APIException($"Channel with Id {request.SMChannelId} is not found");
        }

        List<SMChannelStreamLink> links = Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId).ToList();
        List<SMStreamDto> ret = [];
        foreach (SMStream? stream in channel.SMStreams.Select(a => a.SMStream))
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                SMStreamDto dto = mapper.Map<SMStreamDto>(stream);
                dto.Rank = link.Rank;
                ret.Add(dto);
            }
        }

        return DataResponse<List<SMStreamDto>>.Success(ret.OrderBy(a => a.Rank).ToList());
    }


}
