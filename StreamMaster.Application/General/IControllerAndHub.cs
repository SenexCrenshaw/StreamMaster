using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.General.Commands;

namespace StreamMaster.Application.General
{
    public interface IGeneralController
    {        
        Task<ActionResult<APIResponse>> SetTestTask(SetTestTaskRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IGeneralHub
    {
        Task<APIResponse> SetTestTask(SetTestTaskRequest request);
    }
}
