using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMChannelChannelLinks.Commands;
using StreamMaster.Application.SMChannelChannelLinks.Queries;

namespace StreamMaster.Application.SMChannelChannelLinks
{
    public interface ISMChannelChannelLinksController
    {        
        Task<ActionResult<List<SMChannelDto>>> GetSMChannelChannels(GetSMChannelChannelsRequest request);
        Task<ActionResult<APIResponse>> AddSMChannelToSMChannel(AddSMChannelToSMChannelRequest request);
        Task<ActionResult<APIResponse>> RemoveSMChannelFromSMChannel(RemoveSMChannelFromSMChannelRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelRanks(SetSMChannelRanksRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMChannelChannelLinksHub
    {
        Task<List<SMChannelDto>> GetSMChannelChannels(GetSMChannelChannelsRequest request);
        Task<APIResponse> AddSMChannelToSMChannel(AddSMChannelToSMChannelRequest request);
        Task<APIResponse> RemoveSMChannelFromSMChannel(RemoveSMChannelFromSMChannelRequest request);
        Task<APIResponse> SetSMChannelRanks(SetSMChannelRanksRequest request);
    }
}
