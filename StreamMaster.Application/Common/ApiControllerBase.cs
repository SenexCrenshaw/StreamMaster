using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Application.Common;

[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;
    private ISender _sender = null!;
    private IOptionsMonitor<Setting> _settingsMonitor = null!;
    private IRepositoryWrapper _repositoryWrapper = null!;
    private IStreamGroupService _streamGroupService = null!;
    private ICryptoService _cryptoService = null!;
    private IAPIStatsLogger _aPIStatsLogger = null!;

    protected async Task<T> DebugAPI<T>(Task<T> task, ILogger logger, [CallerMemberName] string callerName = "")
    {
        return await DebugAPIHelper.DebugAPI(task, logger, SettingsMonitor.CurrentValue.DebugAPI, callerName);
    }

    protected IAPIStatsLogger APIStatsLogger =>
    _aPIStatsLogger ??= HttpContext.RequestServices.GetRequiredService<IAPIStatsLogger>();

    protected ICryptoService CryptoService =>
     _cryptoService ??= HttpContext.RequestServices.GetRequiredService<ICryptoService>();

    protected IStreamGroupService StreamGroupService =>
        _streamGroupService ??= HttpContext.RequestServices.GetRequiredService<IStreamGroupService>();

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    protected Setting Settings => SettingsMonitor.CurrentValue;

    /// <summary>
    /// Gets the repository wrapper.
    /// </summary>
    protected IRepositoryWrapper RepositoryWrapper =>
        _repositoryWrapper ??= HttpContext.RequestServices.GetRequiredService<IRepositoryWrapper>();

    /// <summary>
    /// Gets the settings monitor, initializing it if necessary.
    /// </summary>
    private IOptionsMonitor<Setting> SettingsMonitor =>
        _settingsMonitor ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Setting>>();

    /// <summary>
    /// Gets the sender, initializing it if necessary.
    /// </summary>
    protected ISender Sender =>
        _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// Gets the mediator, initializing it if necessary.
    /// </summary>
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
