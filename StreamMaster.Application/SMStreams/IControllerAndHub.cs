using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Application.SMStreams.Queries;

namespace StreamMaster.Application.SMStreams
{
    public interface ISMStreamsController
    {        
        Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMStreamsHub
    {
        Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<APIResponse> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
    }
}
