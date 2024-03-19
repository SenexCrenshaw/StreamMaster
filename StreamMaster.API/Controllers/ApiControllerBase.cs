using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common;

namespace StreamMaster.API.Controllers;

[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;
    private IOptionsMonitor<Setting> _intsettings = null!;
    private IOptionsMonitor<HLSSettings> _inthlssettings = null!;

    protected Setting Settings => intsettings.CurrentValue;
    protected HLSSettings HLSSettings => inthlssettings.CurrentValue;

    protected IOptionsMonitor<Setting> intsettings => _intsettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Setting>>();
    protected IOptionsMonitor<HLSSettings> inthlssettings => _inthlssettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<HLSSettings>>();

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();


}