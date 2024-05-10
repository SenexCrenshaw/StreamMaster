using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Application.SMStreams.Queries;

namespace StreamMaster.Application.SMStreams
{
    public interface ISMStreamsController
    {        
        Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> ToggleSMStreamsVisibleById(ToggleSMStreamsVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMStreamVisibleByParameters(ToggleSMStreamVisibleByParametersRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMStreamsHub
    {
        Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<APIResponse> ToggleSMStreamsVisibleById(ToggleSMStreamsVisibleByIdRequest request);
        Task<APIResponse> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
        Task<APIResponse> ToggleSMStreamVisibleByParameters(ToggleSMStreamVisibleByParametersRequest request);
    }
}
