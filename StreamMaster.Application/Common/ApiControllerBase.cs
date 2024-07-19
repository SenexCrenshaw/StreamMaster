using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;



namespace StreamMaster.Application.Common;

//[ApiController]
[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;
    private ISender _sender = null!;

    private IOptionsMonitor<Setting> _intsettings = null!;
    private IOptionsMonitor<HLSSettings> _inthlssettings = null!;

    private IHubContext<StreamMasterHub, IStreamMasterHub> _intHubContext = null!;
    private IRepositoryWrapper _intRepository = null!;

    protected Setting Settings => intSettings.CurrentValue;
    protected HLSSettings HLSSettings => inthlssettings.CurrentValue;

    protected IHubContext<StreamMasterHub, IStreamMasterHub> HubContext => intHubContext;
    protected IRepositoryWrapper Repository => intRepository;

    protected IOptionsMonitor<Setting> intSettings => _intsettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Setting>>();
    protected IOptionsMonitor<HLSSettings> inthlssettings => _inthlssettings ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<HLSSettings>>();

    protected IHubContext<StreamMasterHub, IStreamMasterHub> intHubContext => _intHubContext ??= HttpContext.RequestServices.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>();
    protected IRepositoryWrapper intRepository => _intRepository ??= HttpContext.RequestServices.GetRequiredService<IRepositoryWrapper>();

    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}