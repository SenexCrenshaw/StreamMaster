using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.EPG.Commands;
using StreamMaster.Application.EPG.Queries;

namespace StreamMaster.Application.EPG
{
    public interface IEPGController
    {        
        Task<ActionResult<List<EPGColorDto>>> GetEPGColors();
        Task<ActionResult<APIResponse?>> EPGSync();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IEPGHub
    {
        Task<List<EPGColorDto>> GetEPGColors();
        Task<APIResponse?> EPGSync();
    }
}
