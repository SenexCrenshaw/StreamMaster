using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Settings.Commands;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    [Authorize]
    public partial class SettingsController(ILogger<SettingsController> _logger) : ApiControllerBase, ISettingsController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<SettingDto>> GetSettings()
        {
            try
            {
                DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSettings.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSettings.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<UpdateSettingResponse?>> UpdateSetting(UpdateSettingRequest request)
        {
            UpdateSettingResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISettingsHub
    {
        public async Task<SettingDto> GetSettings()
        {
            DataResponse<SettingDto> ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<UpdateSettingResponse?> UpdateSetting(UpdateSettingRequest request)
        {
            UpdateSettingResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
