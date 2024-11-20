using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.General.Queries;

namespace StreamMaster.Application.General.Controllers
{
    [Authorize]
    public partial class GeneralController(ILogger<GeneralController> _logger) : ApiControllerBase, IGeneralController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<ImageDownloadServiceStatus>> GetDownloadServiceStatus()
        {
            try
            {
            var ret = await Sender.Send(new GetDownloadServiceStatusRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetDownloadServiceStatus.", statusCode: 500) : Ok(ret.Data?? new());
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
            var ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
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
            var ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSystemStatus.", statusCode: 500) : Ok(ret.Data?? new());
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
            var ret = await Sender.Send(new GetTaskIsRunningRequest()).ConfigureAwait(false);
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
        public async Task<ActionResult<APIResponse?>> SetTestTask(SetTestTaskRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
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
             var ret = await Sender.Send(new GetDownloadServiceStatusRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<bool> GetIsSystemReady()
        {
             var ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<SDSystemStatus> GetSystemStatus()
        {
             var ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<bool> GetTaskIsRunning()
        {
             var ret = await Sender.Send(new GetTaskIsRunningRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse?> SetTestTask(SetTestTaskRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
