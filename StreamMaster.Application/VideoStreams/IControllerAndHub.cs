using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.VideoStreams
{
    public interface IVideoStreamsController
    {        
        Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IVideoStreamsHub
    {
        Task<APIResponse> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request);
        Task<APIResponse> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request);
    }
}
