using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMMessages.Commands;

namespace StreamMaster.Application.SMMessages
{
    public interface ISMMessagesController
    {        
        Task<ActionResult<APIResponse>> SendSMError(SendSMErrorRequest request);
        Task<ActionResult<APIResponse>> SendSMInfo(SendSMInfoRequest request);
        Task<ActionResult<APIResponse>> SendSMMessage(SendSMMessageRequest request);
        Task<ActionResult<APIResponse>> SendSMWarn(SendSMWarnRequest request);
        Task<ActionResult<APIResponse>> SendSuccess(SendSuccessRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMMessagesHub
    {
        Task<APIResponse> SendSMError(SendSMErrorRequest request);
        Task<APIResponse> SendSMInfo(SendSMInfoRequest request);
        Task<APIResponse> SendSMMessage(SendSMMessageRequest request);
        Task<APIResponse> SendSMWarn(SendSMWarnRequest request);
        Task<APIResponse> SendSuccess(SendSuccessRequest request);
    }
}
