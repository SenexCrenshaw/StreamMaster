using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMasterDomain.Common;

namespace StreamMasterAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    //private IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = null!;
    private IMediator _mediator = null!;

    protected ApiControllerBase()
    {
        FileUtil.SetupDirectories();
    }

    //public IHubContext<StreamMasterHub, IStreamMasterHub> HubContext => _hubContext ??= HttpContext.RequestServices.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>();
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
