using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Application.Common;

[V1ApiController("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;
    private ISender _sender = null!;
    private IOptionsMonitor<Setting> _settingsMonitor = null!;
    private IOptionsMonitor<HLSSettings> _hlsSettingsMonitor = null!;
    private IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext = null!;
    private IRepositoryWrapper _repositoryWrapper = null!;
    private IStreamGroupService _streamGroupService = null!;
    private ICryptoService _cryptoService = null!;

    protected ICryptoService CryptoService =>
     _cryptoService ??= HttpContext.RequestServices.GetRequiredService<ICryptoService>();
    protected IStreamGroupService StreamGroupService =>
        _streamGroupService ??= HttpContext.RequestServices.GetRequiredService<IStreamGroupService>();

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    protected Setting Settings => SettingsMonitor.CurrentValue;

    /// <summary>
    /// Gets the current HLS settings.
    /// </summary>
    protected HLSSettings HLSSettings => HlsSettingsMonitor.CurrentValue;

    /// <summary>
    /// Gets the hub context for streaming.
    /// </summary>
    protected IHubContext<StreamMasterHub, IStreamMasterHub> HubContext =>
        _hubContext ??= HttpContext.RequestServices.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>();

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
    /// Gets the HLS settings monitor, initializing it if necessary.
    /// </summary>
    private IOptionsMonitor<HLSSettings> HlsSettingsMonitor =>
        _hlsSettingsMonitor ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<HLSSettings>>();

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
