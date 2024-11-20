using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Logos.Queries;

namespace StreamMaster.Application.Logos
{
    public interface ILogosController
    {
        Task<ActionResult<List<LogoFileDto>>> GetLogos();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ILogosHub
    {
        Task<List<LogoFileDto>> GetLogos();
    }
}
