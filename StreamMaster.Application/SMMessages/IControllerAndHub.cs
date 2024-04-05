using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMMessages.Commands;

namespace StreamMaster.Application.SMMessages
{
    public interface ISMMessagesController
    {        
        Task<ActionResult<DefaultAPIResponse>> SendSMError(SendSMErrorRequest request);
        Task<ActionResult<DefaultAPIResponse>> SendSMInfo(SendSMInfoRequest request);
        Task<ActionResult<DefaultAPIResponse>> SendSMMessage(SendSMMessageRequest request);
        Task<ActionResult<DefaultAPIResponse>> SendSMWarn(SendSMWarnRequest request);
        Task<ActionResult<DefaultAPIResponse>> SendSuccess(SendSuccessRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMMessagesHub
    {
        Task<DefaultAPIResponse> SendSMError(SendSMErrorRequest request);
        Task<DefaultAPIResponse> SendSMInfo(SendSMInfoRequest request);
        Task<DefaultAPIResponse> SendSMMessage(SendSMMessageRequest request);
        Task<DefaultAPIResponse> SendSMWarn(SendSMWarnRequest request);
        Task<DefaultAPIResponse> SendSuccess(SendSuccessRequest request);
    }
}
