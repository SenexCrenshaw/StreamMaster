using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Icons.Queries;

namespace StreamMaster.Application.Icons
{
    public interface IIconsController
    {        
        Task<ActionResult<List<LogoFileDto>>> GetIcons();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IIconsHub
    {
        Task<List<LogoFileDto>> GetIcons();
    }
}
