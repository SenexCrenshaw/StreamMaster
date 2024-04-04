using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Settings.Queries;

namespace StreamMaster.Application.Settings
{
    public partial class SettingsController(ISender Sender) : ApiControllerBase, ISettingsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<bool> GetIsSystemReady()
        {
            bool ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<SettingDto> GetSettings()
        {
            SettingDto ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<SDSystemStatus> GetSystemStatus()
        {
            SDSystemStatus ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISettingsHub
    {
        public async Task<bool> GetIsSystemReady()
        {
            bool ret = await Sender.Send(new GetIsSystemReadyRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<SettingDto> GetSettings()
        {
            SettingDto ret = await Sender.Send(new GetSettingsRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<SDSystemStatus> GetSystemStatus()
        {
            SDSystemStatus ret = await Sender.Send(new GetSystemStatusRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}
