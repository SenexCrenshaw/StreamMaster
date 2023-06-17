using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using StreamMasterInfrastructure.Extensions;
using StreamMasterInfrastructure.Services.Frontend.Mappers;

namespace StreamMasterInfrastructure.Services.Frontend
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
        [HttpGet("/{**path:regex(^(?!(api|feed)/).*)}")]
        public IActionResult Index([FromRoute] string path)
        {
            return MapResource(path);
        }

        [EnableCors("AllowGet")]
        [AllowAnonymous]
        [HttpGet("/content/{**path:regex(^(?!api/).*)}")]
        public IActionResult IndexContent([FromRoute] string path)
        {
            return MapResource("Content/" + path);
        }

        [EnableCors("AllowGet")]
        [AllowAnonymous]
        [HttpGet("/images/{**path:regex(^(?!api/).*)}")]
        public IActionResult IndexImages([FromRoute] string path)
        {
            return MapResource("images/" + path);
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return MapResource("login");
        }

        private IActionResult MapResource(string path)
        {
            path = "/" + (path ?? "");

            var mapper = _requestMappers.SingleOrDefault(m => m.CanHandle(path));

            if (mapper != null)
            {
                var result = mapper.GetResponse(path);

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
