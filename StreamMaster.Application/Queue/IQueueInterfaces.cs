using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common.Models;

namespace StreamMaster.Application.Queue;

public interface IQueueController
{
    Task<ActionResult<List<TaskQueueStatus>>> GetQueueStatus();
}


public interface IQueueHub
{
    Task<List<TaskQueueStatus>> GetQueueStatus();

}