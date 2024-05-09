﻿namespace StreamMaster.Application.StreamGroupSMChannelLinks.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamGroupSMChannelsRequest(int StreamGroupId) : IRequest<DataResponse<List<SMChannelDto>>>;


internal class GetStreamGroupSMChannelsRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetStreamGroupSMChannelsRequest, DataResponse<List<SMChannelDto>>>
{

    public async Task<DataResponse<List<SMChannelDto>>> Handle(GetStreamGroupSMChannelsRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId == null || request.StreamGroupId == 0)
        {
            return DataResponse<List<SMChannelDto>>.ErrorWithMessage($"Streamgroup with Id {request.StreamGroupId} not found");
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup == null)
        {
            return DataResponse<List<SMChannelDto>>.ErrorWithMessage($"Streamgroup with Id {request.StreamGroupId} not found");
        }

        List<StreamGroupSMChannelLink> links = Repository.StreamGroupSMChannelLink.GetQuery(true).Where(a => a.StreamGroupId == request.StreamGroupId).ToList();
        List<SMChannelDto> ret = [];

        foreach (StreamGroupSMChannelLink link in links)
        {
            var channel = link.SMChannel;
            if (link != null)
            {
                SMChannelDto dto = mapper.Map<SMChannelDto>(channel);
                dto.Rank = link.Rank;
                ret.Add(dto);
            }
        }

        return DataResponse<List<SMChannelDto>>.Success(ret.OrderBy(a => a.Rank).ToList());
    }


}
