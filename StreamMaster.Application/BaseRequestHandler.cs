namespace StreamMaster.Application;

//public class BaseRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, IMemoryCache memoryCache)
//{
//    protected readonly ILogger Logger = logger;
//    protected readonly IRepositoryWrapper Repository = repository;
//    protected readonly IMapper Mapper = mapper;
//    protected readonly IMemoryCache MemoryCache = memoryCache;
//    protected Setting Settings => MemoryCache.GetSetting();

//}

//public class BaseMediatorRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseRequestHandler(logger, repository, mapper, memoryCache)
//{
//    protected readonly IHubContext<StreamMasterHub, IStreamMasterHub> HubContext = hubContext;
//    protected readonly IPublisher Publisher = publisher;
//    protected readonly ISender Sender = sender;
//}
