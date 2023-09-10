using StreamMasterApplication.Common.Attributes;

namespace StreamMasterApplication;

[LogExecutionTimeAspect]
public class BaseRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper)
{
    protected readonly ILogger Logger = logger;
    protected readonly IRepositoryWrapper Repository = repository;
    protected readonly IMapper Mapper = mapper;
    protected readonly ISetting Settings = FileUtil.GetSetting();
}

public class BaseMediatorRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseRequestHandler(logger, repository, mapper)
{
    protected readonly IHubContext<StreamMasterHub, IStreamMasterHub> HubContext = hubContext;
    protected readonly IPublisher Publisher = publisher;
    protected readonly ISender Sender = sender;
}

public class BaseMemoryRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext)
{
    protected readonly IMemoryCache MemoryCache = memoryCache;
}
