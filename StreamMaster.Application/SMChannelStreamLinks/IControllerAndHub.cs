using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannelStreamLinks.Commands;
using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks
{
    public interface ISMChannelStreamLinksController
    {        
        Task<ActionResult<List<SMStreamDto>>> GetSMChannelStreams(GetSMChannelStreamsRequest request);
        Task<ActionResult<APIResponse>> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request);
        Task<ActionResult<APIResponse>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request);
        Task<ActionResult<APIResponse>> SetSMStreamRanks(SetSMStreamRanksRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMChannelStreamLinksHub
    {
        Task<List<SMStreamDto>> GetSMChannelStreams(GetSMChannelStreamsRequest request);
        Task<APIResponse> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request);
        Task<APIResponse> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request);
        Task<APIResponse> SetSMStreamRanks(SetSMStreamRanksRequest request);
    }
}
