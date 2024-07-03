using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Configuration;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.EF.Repositories;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure.EF
{
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
        IIconService iconService,
        IMessageService messageService,
        IOptionsMonitor<Setting> intSettings,
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
                _streamGroupProfileRepository ??= new StreamGroupProfileRepository(StreamGroupProfileRepositoryLogger, repositoryContext, intSettings);
                return _streamGroupProfileRepository;
            }
        }

        private IStreamGroupSMChannelLinkRepository _streamGroupSMChannelLinkRepository;
        public IStreamGroupSMChannelLinkRepository StreamGroupSMChannelLink
        {
            get
            {
                _streamGroupSMChannelLinkRepository ??= new StreamGroupSMChannelLinkRepository(StreamGroupSMChannelLinkRepositoryLogger, repositoryContext, this, mapper, intSettings, sender);
                return _streamGroupSMChannelLinkRepository;
            }
        }


        private ISMChannelStreamLinksRepository _smChannelStreamLink;

        public ISMChannelStreamLinksRepository SMChannelStreamLink
        {
            get
            {
                _smChannelStreamLink ??= new SMChannelStreamLinksRepository(SMChannelStreamLinkLogger, repositoryContext, intSettings, mapper);
                return _smChannelStreamLink;
            }
        }


        private ISMChannelsRepository _smChannel;

        public ISMChannelsRepository SMChannel
        {
            get
            {
                _smChannel ??= new SMChannelsRepository(SMChannelLogger, sender, this, repositoryContext, mapper, intSettings, iconService, schedulesDirectDataService);
                return _smChannel;
            }
        }

        private ISMStreamRepository _smStream;

        public ISMStreamRepository SMStream
        {
            get
            {
                _smStream ??= new SMStreamRepository(SMStreamLogger, this, repositoryContext, intSettings, mapper);
                return _smStream;
            }
        }

        private IStreamGroupRepository _streamGroup;

        public IStreamGroupRepository StreamGroup
        {
            get
            {
                _streamGroup ??= new StreamGroupRepository(StreamGroupRepositoryLogger, sender, repositoryContext, mapper, intSettings, httpContextAccessor);
                return _streamGroup;
            }
        }

        private IChannelGroupRepository _channelGroup;

        public IChannelGroupRepository ChannelGroup
        {
            get
            {
                _channelGroup ??= new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, mapper, dataRefreshService, intSettings, sender);
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
}