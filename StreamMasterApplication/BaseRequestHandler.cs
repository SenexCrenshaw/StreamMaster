using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace StreamMasterApplication;

public class BaseRequestHandler
{
    protected readonly ILogger Logger;
    protected readonly IRepositoryWrapper Repository;
    protected readonly IMapper Mapper;

    public BaseRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper)
    {
        Repository = repository;
        Logger = logger;
        Mapper = mapper;
    }
}

public class BaseMediatorRequestHandler : BaseRequestHandler
{

    protected readonly IPublisher Publisher;
    protected readonly ISender Sender;
    public BaseMediatorRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper)
    {
        Publisher = publisher;
        Sender = sender;
    }
}

public class BaseDBRequestHandler : BaseMediatorRequestHandler
{
    protected readonly IAppDbContext Context;
    protected readonly IMemoryCache MemoryCache;

    public BaseDBRequestHandler(ILogger logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IAppDbContext context, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender)
    {

        Context = context;
        MemoryCache = memoryCache;
    }
}