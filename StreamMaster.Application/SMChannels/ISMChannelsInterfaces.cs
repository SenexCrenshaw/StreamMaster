using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.Application.SMChannels;

public interface ISMChannelsController
{
    Task<ActionResult<APIResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] SMChannelParameters Parameters);
}

public interface ISMChannelsHub
{
    Task<APIResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters Parameters);
}