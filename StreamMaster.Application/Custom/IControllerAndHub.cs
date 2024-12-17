using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Custom.Commands;
using StreamMaster.Application.Custom.Queries;

namespace StreamMaster.Application.Custom
{
    public interface ICustomController
    {
        Task<ActionResult<CustomPlayList>> GetCustomPlayList(GetCustomPlayListRequest request);
        Task<ActionResult<List<CustomPlayList>>> GetCustomPlayLists();
        Task<ActionResult<List<CustomPlayList>>> GetIntroPlayLists();
        Task<ActionResult<APIResponse?>> ScanForCustom();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ICustomHub
    {
        Task<CustomPlayList> GetCustomPlayList(GetCustomPlayListRequest request);
        Task<List<CustomPlayList>> GetCustomPlayLists();
        Task<List<CustomPlayList>> GetIntroPlayLists();
        Task<APIResponse?> ScanForCustom();
    }
}
