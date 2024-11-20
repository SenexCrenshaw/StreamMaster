using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Vs.Queries;

namespace StreamMaster.Application.Vs
{
    public interface IVsController
    {
        Task<ActionResult<List<V>>> GetVs(GetVsRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IVsHub
    {
        Task<List<V>> GetVs(GetVsRequest request);
    }
}
