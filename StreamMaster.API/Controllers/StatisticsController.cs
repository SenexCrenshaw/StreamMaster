using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Statistics;
using StreamMaster.Application.Statistics.Queries;
using StreamMaster.Streams.Domain.Models;

using StreamMasterAPI.Controllers;

namespace StreamMaster.API.Controllers;

public class StatisticsController : ApiControllerBase, IStatisticsController
{

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<ClientStreamingStatistics>>> GetClientStatistics()
    {
        List<ClientStreamingStatistics> res = await Mediator.Send(new GetClientStreamingStatistics()).ConfigureAwait(false);
        return Ok(res);
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<ActionResult<List<InputStreamingStatistics>>> GetInputStatistics()
    {
        List<InputStreamingStatistics> res = await Mediator.Send(new GetInputStatistics()).ConfigureAwait(false);
        return Ok(res);
    }
}