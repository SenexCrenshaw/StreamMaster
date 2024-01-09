using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Common;
using StreamMaster.Infrastructure;



namespace StreamMaster.API.Controllers;

//[ApiController]
[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{

    //private IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = null!;
    private IMediator _mediator = null!;
    private IMemoryCache memoryCache = null!;


    protected Setting Settings => MemoryCache.GetSetting();

    protected IMemoryCache MemoryCache => memoryCache ??= HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}