using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.Application.Statistics;

public interface IStatisticsController
{
    Task<ActionResult<List<ClientStreamingStatistics>>> GetClientStatistics();
    Task<ActionResult<List<InputStreamingStatistics>>> GetInputStatistics();
}


public interface IStatisticsub
{
    Task<List<ClientStreamingStatistics>> GetClientStatistics();
    Task<List<InputStreamingStatistics>> GetInputStatistics();
}
