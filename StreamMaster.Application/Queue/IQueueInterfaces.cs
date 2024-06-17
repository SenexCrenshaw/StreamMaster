using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.Application.Queue;

public interface IQueueController
{
    Task<ActionResult<List<SMTask>>> GetQueueStatus();
}


public interface IQueueHub
{
    Task<List<SMTask>> GetQueueStatus();

}