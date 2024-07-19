using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.CustomPlayLists.Commands;
using StreamMaster.Application.CustomPlayLists.Queries;
using StreamMaster.PlayList.Models;

namespace StreamMaster.Application.CustomPlayLists
{
    public interface ICustomPlayListsController
    {        
        Task<ActionResult<CustomPlayList>> GetCustomPlayList(GetCustomPlayListRequest request);
        Task<ActionResult<List<CustomPlayList>>> GetCustomPlayLists();
        Task<ActionResult<APIResponse>> ScanForCustomPlayLists();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ICustomPlayListsHub
    {
        Task<CustomPlayList> GetCustomPlayList(GetCustomPlayListRequest request);
        Task<List<CustomPlayList>> GetCustomPlayLists();
        Task<APIResponse> ScanForCustomPlayLists();
    }
}
