using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPG.Queries;

namespace StreamMaster.Application.EPG
{
    public interface IEPGController
    {        
        Task<ActionResult<List<EPGColorDto>>> GetEPGColors();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IEPGHub
    {
        Task<List<EPGColorDto>> GetEPGColors();
    }
}
