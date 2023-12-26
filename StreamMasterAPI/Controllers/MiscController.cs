using Microsoft.AspNetCore.Mvc;



using StreamMasterDomain.Services;

namespace StreamMasterAPI.Controllers;

public class MiscController : ApiControllerBase
{
    private readonly IImageDownloadService imageDownloadService;
    
    public MiscController(IImageDownloadService imageDownloadService)
    {
        this.imageDownloadService = imageDownloadService;        
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult<ImageDownloadServiceStatus> GetDownloadServiceStatus()
    {
        var status = imageDownloadService.GetStatus();
        //var json = System.Text.Json.JsonSerializer.Serialize(status);
        //return new ContentResult
        //{
        //    Content = json,
        //    ContentType = "text/json",
        //    StatusCode = 200
        //};
        return Ok(status);
    }

    //[HttpPatch]
    //[Route("[action]")]
    //public async Task<ActionResult> BuildIconsCacheFromVideoStreams()
    //{
    //    await _taskQueue.BuildIconsCacheFromVideoStreams().ConfigureAwait(false);
    //    return NoContent();
    //}




    //[HttpPatch]
    //[Route("[action]")]
    //public async Task<ActionResult> BuildProgIconsCacheFromEPGsRequest()
    //{
    //    await _sender.Send(new BuildProgIconsCacheFromEPGsRequest()).ConfigureAwait(false);

    //    return NoContent();
    //}
}