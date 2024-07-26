using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StreamMaster.Infrastructure.Extensions;
using StreamMaster.Infrastructure.Services.Frontend.Mappers;

namespace StreamMaster.Infrastructure.Services.Frontend
{
    [Authorize(Policy = "UI")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class StaticResourceController : Controller
    {
        private readonly ILogger<StaticResourceController> _logger;
        private readonly IEnumerable<IMapHttpRequestsToDisk> _requestMappers;

        public StaticResourceController(IEnumerable<IMapHttpRequestsToDisk> requestMappers, ILogger<StaticResourceController> logger)
        {
            _requestMappers = requestMappers;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("/{**path:regex(^(?!(m|V|api|feed)/).*)}")]
        public async Task<IActionResult> Index([FromRoute] string path)
        {
            return await MapResource(path);
        }

        [EnableCors("AllowGet")]
        [AllowAnonymous]
        [HttpGet("/content/{**path:regex(^(?!api/).*)}")]
        public async Task<IActionResult> IndexContent([FromRoute] string path)
        {
            return await MapResource("Content/" + path);
        }

        [HttpGet("/swagger/{**path:regex(^(?!api/).*)}")]
        public async Task<IActionResult> IndexSwagger([FromRoute] string path)
        {
            return await MapResource("swagger/" + path);
        }

        [EnableCors("AllowGet")]
        [AllowAnonymous]
        [HttpGet("/images/{**path:regex(^(?!api/).*)}")]
        public async Task<IActionResult> IndexImages([FromRoute] string path)
        {
            return await MapResource("images/" + path);
        }


        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<IActionResult> LoginPage()
        {
            return await MapResource("login");
        }

        private async Task<IActionResult> MapResource(string path)
        {
            path = "/" + (path ?? "");

            IMapHttpRequestsToDisk? mapper = _requestMappers.SingleOrDefault(m => m.CanHandle(path));

            if (mapper != null)
            {
                IActionResult result = await mapper.GetResponse(path);

                if (result != null)
                {
                    if ((result as FileResult)?.ContentType == "text/html")
                    {
                        Response.Headers.DisableCache();
                    }

                    return result;
                }

                return NotFound();
            }

            _logger.LogWarning("Couldn't find handler for {0}", path);

            return NotFound();
        }
    }
}
