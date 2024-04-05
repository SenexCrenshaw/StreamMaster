using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    public partial class SettingsController(ISender Sender, ILogger<SettingsController> _logger) : ApiControllerBase, ISettingsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<bool>> GetIsSystemReady()
        {
            try
            {
            APIResponse<bool> ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
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
        public async Task<ActionResult<SettingDto>> GetSettings()
        {
            try
            {
            APIResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSettings.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSettings.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SDSystemStatus>> GetSystemStatus()
        {
            try
            {
            APIResponse<SDSystemStatus> ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSystemStatus.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSystemStatus.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISettingsHub
    {
        public async Task<bool> GetIsSystemReady()
        {
             APIResponse<bool> ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<SettingDto> GetSettings()
        {
             APIResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<SDSystemStatus> GetSystemStatus()
        {
             APIResponse<SDSystemStatus> ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
