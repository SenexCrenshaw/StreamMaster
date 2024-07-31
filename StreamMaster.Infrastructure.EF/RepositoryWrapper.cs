using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Configuration;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.EF.Repositories;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.EF;

public class RepositoryWrapper(
     ILogger<ChannelGroupRepository> ChannelGroupRepositoryLogger,
    ILogger<StreamGroupRepository> StreamGroupRepositoryLogger,
    ILogger<M3UFileRepository> M3UFileRepositoryLogger,
    ILogger<EPGFileRepository> EPGFileRepositoryLogger,
    ILogger<SMChannelsRepository> SMChannelLogger,
    ILogger<StreamGroupProfileRepository> StreamGroupProfileRepositoryLogger,
    ILogger<SMStreamRepository> SMStreamLogger,
    ILogger<SMChannelStreamLinksRepository> SMChannelStreamLinkLogger,
    ILogger<StreamGroupSMChannelLinkRepository> StreamGroupSMChannelLinkRepositoryLogger,
    ISchedulesDirectDataService schedulesDirectDataService,
    PGSQLRepositoryContext repositoryContext,
    IMapper mapper,
    IXmltv2Mxf xmltv2Mxf,
    IMessageService messageService,
    IOptionsMonitor<Setting> intSettings,
    IOptionsMonitor<CommandProfiles> intProfileSettings,
    IJobStatusService jobStatusService,
    ISender sender,
    IDataRefreshService dataRefreshService,
    IHttpContextAccessor httpContextAccessor) : IRepositoryWrapper
{
    private IStreamGroupProfileRepository _streamGroupProfileRepository;
    public IStreamGroupProfileRepository StreamGroupProfile
    {
        get
        {
            _streamGroupProfileRepository ??= new StreamGroupProfileRepository(StreamGroupProfileRepositoryLogger, repositoryContext);
            return _streamGroupProfileRepository;
        }
    }

    private IStreamGroupSMChannelLinkRepository _streamGroupSMChannelLinkRepository;
    public IStreamGroupSMChannelLinkRepository StreamGroupSMChannelLink
    {
        get
        {
            _streamGroupSMChannelLinkRepository ??= new StreamGroupSMChannelLinkRepository(StreamGroupSMChannelLinkRepositoryLogger, repositoryContext, this);
            return _streamGroupSMChannelLinkRepository;
        }
    }

    private ISMChannelStreamLinksRepository _smChannelStreamLink;

    public ISMChannelStreamLinksRepository SMChannelStreamLink
    {
        get
        {
            _smChannelStreamLink ??= new SMChannelStreamLinksRepository(SMChannelStreamLinkLogger, repositoryContext);
            return _smChannelStreamLink;
        }
    }

    private ISMChannelsRepository _smChannel;

    public ISMChannelsRepository SMChannel
    {
        get
        {
            _smChannel ??= new SMChannelsRepository(SMChannelLogger, this, repositoryContext, mapper, intSettings, intProfileSettings, schedulesDirectDataService);
            return _smChannel;
        }
    }

    private ISMStreamRepository _smStream;

    public ISMStreamRepository SMStream
    {
        get
        {
            _smStream ??= new SMStreamRepository(SMStreamLogger, this, repositoryContext, mapper);
            return _smStream;
        }
    }

    private IStreamGroupRepository _streamGroup;

    public IStreamGroupRepository StreamGroup
    {
        get
        {
            _streamGroup ??= new StreamGroupRepository(StreamGroupRepositoryLogger, sender, this, repositoryContext, mapper, intSettings, httpContextAccessor);
            return _streamGroup;
        }
    }

    private IChannelGroupRepository _channelGroup;

    public IChannelGroupRepository ChannelGroup
    {
        get
        {
            _channelGroup ??= new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, dataRefreshService);
            return _channelGroup;
        }
    }

    private IM3UFileRepository _m3uFile;

    public IM3UFileRepository M3UFile
    {
        get
        {
            _m3uFile ??= new M3UFileRepository(M3UFileRepositoryLogger, messageService, this, jobStatusService, repositoryContext, intSettings, mapper);
            return _m3uFile;
        }
    }

    private IEPGFileRepository _epgFile;

    public IEPGFileRepository EPGFile
    {
        get
        {
            _epgFile ??= new EPGFileRepository(EPGFileRepositoryLogger, xmltv2Mxf, jobStatusService, repositoryContext, schedulesDirectDataService, intSettings, mapper);
            return _epgFile;
        }
    }

    public async Task<int> SaveAsync()
    {
        return await repositoryContext.SaveChangesAsync();
    }
}