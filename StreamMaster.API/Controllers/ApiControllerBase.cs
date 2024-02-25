using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Configuration;
using StreamMaster.Infrastructure;



namespace StreamMaster.API.Controllers;

//[ApiController]
[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{

    //private IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = null!;
    private IMediator _mediator = null!;
    private IOptionsMonitor<Setting> _intsettings = null!;

    protected Setting Settings => intsettings.CurrentValue;

    protected IOptionsMonitor<Setting> intsettings => _intsettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Setting>>();
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}