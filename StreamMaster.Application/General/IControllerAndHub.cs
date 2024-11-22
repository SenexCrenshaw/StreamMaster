using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.General.Queries;

namespace StreamMaster.Application.General
{
    public interface IGeneralController
    {        
        Task<ActionResult<ImageDownloadServiceStatus>> GetDownloadServiceStatus();
        Task<ActionResult<bool>> GetIsSystemReady();
        Task<ActionResult<SDSystemStatus>> GetSystemStatus();
        Task<ActionResult<bool>> GetTaskIsRunning();
        Task<ActionResult<APIResponse?>> SetTestTask(SetTestTaskRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IGeneralHub
    {
        Task<ImageDownloadServiceStatus> GetDownloadServiceStatus();
        Task<bool> GetIsSystemReady();
        Task<SDSystemStatus> GetSystemStatus();
        Task<bool> GetTaskIsRunning();
        Task<APIResponse?> SetTestTask(SetTestTaskRequest request);
    }
}
