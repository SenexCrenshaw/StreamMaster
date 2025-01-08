using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.Configuration;
using StreamMaster.EPG;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.EF.Repositories;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.Infrastructure.EF;

public class RepositoryWrapper(
     ILogger<ChannelGroupRepository> ChannelGroupRepositoryLogger,
    ILogger<StreamGroupRepository> StreamGroupRepositoryLogger,
    ILogger<M3UFileRepository> M3UFileRepositoryLogger,
    ILogger<EPGFileRepository> EPGFileRepositoryLogger,
    ILogger<SMChannelsRepository> SMChannelLogger,
    ILogger<StreamGroupProfileRepository> StreamGroupProfileRepositoryLogger,
    ILogger<SMStreamRepository> SMStreamLogger,
    ILogger<SMChannelChannelLinksRepository> SMChannelChannelLinkLogger,
    ILogger<SMChannelStreamLinksRepository> SMChannelStreamLinkLogger,
    ILogger<StreamGroupSMChannelLinkRepository> StreamGroupSMChannelLinkRepositoryLogger,
    PGSQLRepositoryContext repositoryContext,
    IMapper mapper,
    ICacheManager cacheManager,
    ISender sender,
    IEpgMatcher epgMatcher,
    IServiceProvider serviceProvider,
    ICryptoService cryptoService,
    ILogoService logoService,
    IOptionsMonitor<Setting> intSettings,
    IOptionsMonitor<CommandProfileDict> intProfileSettings,
    IJobStatusService jobStatusService,
    IFileUtilService fileUtilService,
    IImageDownloadService imageDownloadService,
    IDataRefreshService dataRefreshService,
    IImageDownloadQueue imageDownloadQueue,
    IHttpContextAccessor httpContextAccessor) : IRepositoryWrapper
{
    private IStreamGroupProfileRepository? _streamGroupProfileRepository;

    public IStreamGroupProfileRepository StreamGroupProfile
    {
        get
        {
            _streamGroupProfileRepository ??= new StreamGroupProfileRepository(StreamGroupProfileRepositoryLogger, repositoryContext);
            return _streamGroupProfileRepository;
        }
    }

    private IStreamGroupSMChannelLinkRepository? _streamGroupSMChannelLinkRepository;

    public IStreamGroupSMChannelLinkRepository StreamGroupSMChannelLink
    {
        get
        {
            _streamGroupSMChannelLinkRepository ??= new StreamGroupSMChannelLinkRepository(StreamGroupSMChannelLinkRepositoryLogger, repositoryContext, this);
            return _streamGroupSMChannelLinkRepository;
        }
    }

    private ISMChannelChannelLinksRepository? _smChannelChannelLink;

    public ISMChannelChannelLinksRepository SMChannelChannelLink
    {
        get
        {
            _smChannelChannelLink ??= new SMChannelChannelLinksRepository(SMChannelChannelLinkLogger, repositoryContext);
            return _smChannelChannelLink;
        }
    }

    private ISMChannelStreamLinksRepository? _smChannelStreamLink;

    public ISMChannelStreamLinksRepository SMChannelStreamLink
    {
        get
        {
            _smChannelStreamLink ??= new SMChannelStreamLinksRepository(SMChannelStreamLinkLogger, repositoryContext);
            return _smChannelStreamLink;
        }
    }

    private ISMChannelsRepository? _smChannel;

    public ISMChannelsRepository SMChannel
    {
        get
        {
            _smChannel ??= new SMChannelsRepository(SMChannelLogger, sender, epgMatcher, logoService, cacheManager, imageDownloadService, imageDownloadQueue, serviceProvider, this, repositoryContext, mapper, intSettings, intProfileSettings);
            return _smChannel;
        }
    }

    private ISMStreamRepository? _smStream;

    public ISMStreamRepository SMStream
    {
        get
        {
            _smStream ??= new SMStreamRepository(SMStreamLogger, this, repositoryContext, mapper);
            return _smStream;
        }
    }

    private IStreamGroupRepository? _streamGroup;

    public IStreamGroupRepository StreamGroup
    {
        get
        {
            _streamGroup ??= new StreamGroupRepository(StreamGroupRepositoryLogger, this, repositoryContext, mapper, intSettings, cryptoService, httpContextAccessor);
            return _streamGroup;
        }
    }

    private IChannelGroupRepository? _channelGroup;

    public IChannelGroupRepository ChannelGroup
    {
        get
        {
            _channelGroup ??= new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, dataRefreshService);
            return _channelGroup;
        }
    }

    private IM3UFileRepository? _m3uFile;

    public IM3UFileRepository M3UFile
    {
        get
        {
            _m3uFile ??= new M3UFileRepository(M3UFileRepositoryLogger, repositoryContext, mapper);
            return _m3uFile;
        }
    }

    private IEPGFileRepository? _epgFile;

    public IEPGFileRepository EPGFile
    {
        get
        {
            _epgFile ??= new EPGFileRepository(EPGFileRepositoryLogger, fileUtilService, cacheManager, jobStatusService, repositoryContext, mapper);
            return _epgFile;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await repositoryContext.SaveChangesAsync();
    }
}