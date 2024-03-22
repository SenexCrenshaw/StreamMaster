using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;


public record UpdateStreamRanksRequest(int SMChannelId, List<SMStream> streams) : IRequest<List<SMStreamDto>>;

internal class UpdateStreamRanksRequestHandler(IRepositoryWrapper Repository, IMapper mapper, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateStreamRanksRequest, List<SMStreamDto>>
{
    public async Task<List<SMStreamDto>> Handle(UpdateStreamRanksRequest request, CancellationToken cancellationToken)
    {
        List<SMStreamDto> ret = [];

        List<SMChannelStreamLink> links = [.. Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId)];

        foreach (SMStream stream in request.streams)
        {
            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                SMStreamDto sm = mapper.Map<SMStreamDto>(stream);
                sm.Rank = link.Rank;
                ret.Add(sm);
            }
        }
        return ret;
    }
}
