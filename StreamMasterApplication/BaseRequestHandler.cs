namespace StreamMasterApplication;

public class BaseRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService)
{
    protected readonly ILogger Logger = logger;
    protected readonly IRepositoryWrapper Repository = repository;
    protected readonly IMapper Mapper = mapper;
    private readonly ISettingsService SettingsService = settingsService;
    public async Task<Setting> GetSettingsAsync()
    {
        Setting settings = await SettingsService.GetSettingsAsync();
        return settings;
    }
}

public class BaseMediatorRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseRequestHandler(logger, repository, mapper, settingsService)
{
    protected readonly IHubContext<StreamMasterHub, IStreamMasterHub> HubContext = hubContext;
    protected readonly IPublisher Publisher = publisher;
    protected readonly ISender Sender = sender;
    protected readonly IMemoryCache MemoryCache = memoryCache;
}
