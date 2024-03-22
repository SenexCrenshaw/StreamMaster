using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SMStreams.Commands;

namespace StreamMaster.Application.SMStreams
{
    public interface ISMStreamsController
    {        
    Task<ActionResult<APIResponse<SMStreamDto>>> GetPagedSMStreams(SMStreamParameters Parameters);
    Task<ActionResult<DefaultAPIResponse?>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMStreamsHub
    {
        Task<APIResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters Parameters);
        Task<DefaultAPIResponse?> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
    }
}
