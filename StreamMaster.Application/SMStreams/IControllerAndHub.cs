using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Application.SMStreams.Queries;

namespace StreamMaster.Application.SMStreams
{
    public interface ISMStreamsController
    {        
        Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> CreateSMStream(CreateSMStreamRequest request);
        Task<ActionResult<APIResponse>> DeleteSMStream(DeleteSMStreamRequest request);
        Task<ActionResult<APIResponse>> SetSMStreamsVisibleById(SetSMStreamsVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMStreamsVisibleById(ToggleSMStreamsVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMStreamVisibleByParameters(ToggleSMStreamVisibleByParametersRequest request);
        Task<ActionResult<APIResponse>> UpdateSMStream(UpdateSMStreamRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMStreamsHub
    {
        Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters Parameters);
        Task<APIResponse> CreateSMStream(CreateSMStreamRequest request);
        Task<APIResponse> DeleteSMStream(DeleteSMStreamRequest request);
        Task<APIResponse> SetSMStreamsVisibleById(SetSMStreamsVisibleByIdRequest request);
        Task<APIResponse> ToggleSMStreamsVisibleById(ToggleSMStreamsVisibleByIdRequest request);
        Task<APIResponse> ToggleSMStreamVisibleById(ToggleSMStreamVisibleByIdRequest request);
        Task<APIResponse> ToggleSMStreamVisibleByParameters(ToggleSMStreamVisibleByParametersRequest request);
        Task<APIResponse> UpdateSMStream(UpdateSMStreamRequest request);
    }
}
