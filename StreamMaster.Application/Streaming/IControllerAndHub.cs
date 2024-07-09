using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Streaming.Commands;
using StreamMaster.Application.Streaming.Queries;

namespace StreamMaster.Application.Streaming
{
    public interface IStreamingController
    {        
        Task<ActionResult<List<IdNameUrl>>> GetVideoStreamNamesAndUrls();
        Task<ActionResult<APIResponse>> CancelAllChannels();
        Task<ActionResult<APIResponse>> CancelChannel(CancelChannelRequest request);
        Task<ActionResult<APIResponse>> CancelClient(CancelClientRequest request);
        Task<ActionResult<APIResponse>> MoveToNextStream(MoveToNextStreamRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStreamingHub
    {
        Task<List<IdNameUrl>> GetVideoStreamNamesAndUrls();
        Task<APIResponse> CancelAllChannels();
        Task<APIResponse> CancelChannel(CancelChannelRequest request);
        Task<APIResponse> CancelClient(CancelClientRequest request);
        Task<APIResponse> MoveToNextStream(MoveToNextStreamRequest request);
    }
}
