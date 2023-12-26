using MediatR;

using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Services;

using StreamMaster.Infrastructure;



namespace StreamMasterAPI.Controllers;

//[ApiController]
[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{

    //private IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = null!;
    private IMediator _mediator = null!;
    private ISettingsService _settingsService = null!;

    protected ISettingsService SettingsService => _settingsService ??= HttpContext.RequestServices.GetRequiredService<ISettingsService>();

    //protected Setting Settings => _settingsService ??= HttpContext.RequestServices.GetRequiredService<ISettingsService>();

    //protected ApiControllerBase()
    //{
    //    FileUtil.SetupDirectories();
    //}

    //public IHubContext<StreamMasterHub, IStreamMasterHub> HubContext => _hubContext ??= HttpContext.RequestServices.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>();
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}