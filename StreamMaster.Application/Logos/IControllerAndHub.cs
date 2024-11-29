using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logos.Commands;
using StreamMaster.Application.Logos.Queries;

namespace StreamMaster.Application.Logos
{
    public interface ILogosController
    {
        Task<ActionResult<List<CustomLogoDto>>> GetCustomLogos();
        Task<ActionResult<LogoDto>> GetLogoForChannel(GetLogoForChannelRequest request);
        Task<ActionResult<LogoDto>> GetLogo(GetLogoRequest request);
        Task<ActionResult<List<CustomLogoDto>>> GetLogos();
        Task<ActionResult<APIResponse?>> RemoveCustomLogo(RemoveCustomLogoRequest request);
        Task<ActionResult<APIResponse?>> AddCustomLogo(AddCustomLogoRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ILogosHub
    {
        Task<List<CustomLogoDto>> GetCustomLogos();
        Task<LogoDto> GetLogoForChannel(GetLogoForChannelRequest request);
        Task<LogoDto> GetLogo(GetLogoRequest request);
        Task<List<CustomLogoDto>> GetLogos();
        Task<APIResponse?> RemoveCustomLogo(RemoveCustomLogoRequest request);
        Task<APIResponse?> AddCustomLogo(AddCustomLogoRequest request);
    }
}
