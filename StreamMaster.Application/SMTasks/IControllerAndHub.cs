using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMTasks.Commands;
using StreamMaster.Application.SMTasks.Queries;

namespace StreamMaster.Application.SMTasks
{
    public interface ISMTasksController
    {
        Task<ActionResult<List<SMTask>>> GetSMTasks();
        Task<ActionResult<APIResponse?>> SendSMTasks(SendSMTasksRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMTasksHub
    {
        Task<List<SMTask>> GetSMTasks();
        Task<APIResponse?> SendSMTasks(SendSMTasksRequest request);
    }
}
