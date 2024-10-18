using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logs.Queries;

namespace StreamMaster.Application.Logs
{
    public interface ILogsController
    {        
        Task<ActionResult<string>> GetLogContents(GetLogContentsRequest request);
        Task<ActionResult<List<string>>> GetLogNames();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ILogsHub
    {
        Task<string> GetLogContents(GetLogContentsRequest request);
        Task<List<string>> GetLogNames();
    }
}
