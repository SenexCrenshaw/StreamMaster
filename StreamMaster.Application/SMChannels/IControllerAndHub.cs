using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels.Commands;
using StreamMaster.Application.SMChannels.Queries;

namespace StreamMaster.Application.SMChannels
{
    public interface ISMChannelsController
    {        
        Task<ActionResult<PagedResponse<SMChannelDto>>> GetPagedSMChannels(QueryStringParameters Parameters);
        Task<ActionResult<List<string>>> GetSMChannelNames();
        Task<ActionResult<SMChannelDto>> GetSMChannel(GetSMChannelRequest request);
        Task<ActionResult<VideoInfo>> GetVideoInfoFromId(GetVideoInfoFromIdRequest request);
        Task<ActionResult<APIResponse>> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
        Task<ActionResult<APIResponse>> AutoSetEPG(AutoSetEPGRequest request);
        Task<ActionResult<APIResponse>> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request);
        Task<ActionResult<APIResponse>> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request);
        Task<ActionResult<APIResponse>> CopySMChannel(CopySMChannelRequest request);
        Task<ActionResult<APIResponse>> CreateSMChannel(CreateSMChannelRequest request);
        Task<ActionResult<APIResponse>> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request);
        Task<ActionResult<APIResponse>> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request);
        Task<ActionResult<APIResponse>> DeleteSMChannel(DeleteSMChannelRequest request);
        Task<ActionResult<APIResponse>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request);
        Task<ActionResult<APIResponse>> DeleteSMChannels(DeleteSMChannelsRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelEPGId(SetSMChannelEPGIdRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelGroup(SetSMChannelGroupRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelLogo(SetSMChannelLogoRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelName(SetSMChannelNameRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelNumber(SetSMChannelNumberRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsGroup(SetSMChannelsGroupRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsVideoOutputProfileNameFromParameters(SetSMChannelsVideoOutputProfileNameFromParametersRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelsVideoOutputProfileName(SetSMChannelsVideoOutputProfileNameRequest request);
        Task<ActionResult<APIResponse>> SetSMChannelVideoOutputProfileName(SetSMChannelVideoOutputProfileNameRequest request);
        Task<ActionResult<APIResponse>> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request);
        Task<ActionResult<APIResponse>> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request);
        Task<ActionResult<APIResponse>> UpdateSMChannel(UpdateSMChannelRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMChannelsHub
    {
        Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters Parameters);
        Task<List<string>> GetSMChannelNames();
        Task<SMChannelDto> GetSMChannel(GetSMChannelRequest request);
        Task<VideoInfo> GetVideoInfoFromId(GetVideoInfoFromIdRequest request);
        Task<APIResponse> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request);
        Task<APIResponse> AutoSetEPG(AutoSetEPGRequest request);
        Task<APIResponse> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request);
        Task<APIResponse> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request);
        Task<APIResponse> CopySMChannel(CopySMChannelRequest request);
        Task<APIResponse> CreateSMChannel(CreateSMChannelRequest request);
        Task<APIResponse> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request);
        Task<APIResponse> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request);
        Task<APIResponse> DeleteSMChannel(DeleteSMChannelRequest request);
        Task<APIResponse> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request);
        Task<APIResponse> DeleteSMChannels(DeleteSMChannelsRequest request);
        Task<APIResponse> SetSMChannelEPGId(SetSMChannelEPGIdRequest request);
        Task<APIResponse> SetSMChannelGroup(SetSMChannelGroupRequest request);
        Task<APIResponse> SetSMChannelLogo(SetSMChannelLogoRequest request);
        Task<APIResponse> SetSMChannelName(SetSMChannelNameRequest request);
        Task<APIResponse> SetSMChannelNumber(SetSMChannelNumberRequest request);
        Task<APIResponse> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request);
        Task<APIResponse> SetSMChannelsGroup(SetSMChannelsGroupRequest request);
        Task<APIResponse> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request);
        Task<APIResponse> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request);
        Task<APIResponse> SetSMChannelsVideoOutputProfileNameFromParameters(SetSMChannelsVideoOutputProfileNameFromParametersRequest request);
        Task<APIResponse> SetSMChannelsVideoOutputProfileName(SetSMChannelsVideoOutputProfileNameRequest request);
        Task<APIResponse> SetSMChannelVideoOutputProfileName(SetSMChannelVideoOutputProfileNameRequest request);
        Task<APIResponse> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request);
        Task<APIResponse> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request);
        Task<APIResponse> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request);
        Task<APIResponse> UpdateSMChannel(UpdateSMChannelRequest request);
    }
}
