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
    private IOptionsMonitor<HLSSettings> _inthlssettings = null!;

    protected Setting Settings => intsettings.CurrentValue;
    protected HLSSettings HLSSettings => inthlssettings.CurrentValue;

    protected IOptionsMonitor<Setting> intsettings => _intsettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Setting>>();
    protected IOptionsMonitor<HLSSettings> inthlssettings => _inthlssettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<HLSSettings>>();

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}