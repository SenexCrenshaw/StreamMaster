using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.General.Queries;

namespace StreamMaster.Application.General.Controllers
{
    public partial class GeneralController(ILogger<GeneralController> _logger) : ApiControllerBase, IGeneralController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ImageDownloadServiceStatus>> GetDownloadServiceStatus()
        {
            try
            {
            DataResponse<ImageDownloadServiceStatus> ret = await Sender.Send(new GetDownloadServiceStatusRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetDownloadServiceStatus.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetDownloadServiceStatus.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<bool>> GetIsSystemReady()
        {
            try
            {
            DataResponse<bool> ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetIsSystemReady.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetIsSystemReady.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SDSystemStatus>> GetSystemStatus()
        {
            try
            {
            DataResponse<SDSystemStatus> ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSystemStatus.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSystemStatus.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<bool>> GetTaskIsRunning()
        {
            try
            {
            DataResponse<bool> ret = await Sender.Send(new GetTaskIsRunningRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetTaskIsRunning.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetTaskIsRunning.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetTestTask(SetTestTaskRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IGeneralHub
    {
        public async Task<ImageDownloadServiceStatus> GetDownloadServiceStatus()
        {
             DataResponse<ImageDownloadServiceStatus> ret = await Sender.Send(new GetDownloadServiceStatusRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<bool> GetIsSystemReady()
        {
             DataResponse<bool> ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<SDSystemStatus> GetSystemStatus()
        {
             DataResponse<SDSystemStatus> ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<bool> GetTaskIsRunning()
        {
             DataResponse<bool> ret = await Sender.Send(new GetTaskIsRunningRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> SetTestTask(SetTestTaskRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
