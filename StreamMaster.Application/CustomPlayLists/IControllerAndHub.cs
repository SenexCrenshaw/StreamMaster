using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.CustomPlayLists.Commands;

namespace StreamMaster.Application.CustomPlayLists
{
    public interface ICustomPlayListsController
    {        
        Task<ActionResult<APIResponse>> ScanForCustomPlayLists();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ICustomPlayListsHub
    {
        Task<APIResponse> ScanForCustomPlayLists();
    }
}
