using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.SMChannels
{
    public interface ISMChannelsController
    {        
    Task<ActionResult<DefaultAPIResponse>> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request);
    Task<ActionResult<DefaultAPIResponse>> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request);
    Task<ActionResult<DefaultAPIResponse>> DeleteSMChannel(DeleteSMChannelRequest request);
    Task<ActionResult<DefaultAPIResponse>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request);
    Task<ActionResult<DefaultAPIResponse>> DeleteSMChannels(DeleteSMChannelsRequest request);
    Task<ActionResult<APIResponse<SMChannelDto>>> GetPagedSMChannels(SMChannelParameters Parameters);
    Task<ActionResult<DefaultAPIResponse>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request);
    Task<ActionResult<DefaultAPIResponse>> SetSMChannelLogo(SetSMChannelLogoRequest request);
    Task<ActionResult<DefaultAPIResponse>> SetSMStreamRanks(SetSMStreamRanksRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMChannelsHub
    {
        Task<DefaultAPIResponse> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request);
        Task<DefaultAPIResponse> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request);
        Task<DefaultAPIResponse> DeleteSMChannel(DeleteSMChannelRequest request);
        Task<DefaultAPIResponse> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request);
        Task<DefaultAPIResponse> DeleteSMChannels(DeleteSMChannelsRequest request);
        Task<APIResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters Parameters);
        Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request);
        Task<DefaultAPIResponse> SetSMChannelLogo(SetSMChannelLogoRequest request);
        Task<DefaultAPIResponse> SetSMStreamRanks(SetSMStreamRanksRequest request);
    }
}
