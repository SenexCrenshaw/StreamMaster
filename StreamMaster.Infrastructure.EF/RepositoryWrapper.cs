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
        ILogger<VideoStreamLinkRepository> VideoStreamLinkRepositoryLogger,
        ILogger<EPGFileRepository> EPGFileRepositoryLogger,
        ILogger<VideoStreamRepository> VideoStreamRepositoryLogger,
        ILogger<SMChannelsRepository> SMChannelLogger,
        ILogger<SMStreamRepository> SMStreamLogger,
        ILogger<SMChannelStreamLinksRepository> SMChannelStreamLinkLogger,
        ILogger<StreamGroupVideoStreamRepository> StreamGroupVideoStreamRepositoryLogger,
        ILogger<StreamGroupChannelGroupRepository> StreamGroupChannelGroupRepositoryLogger,
        ISchedulesDirectDataService schedulesDirectDataService,
        PGSQLRepositoryContext repositoryContext,
        IMapper mapper,
        IXmltv2Mxf xmltv2Mxf,
        IIconService iconService,
        IOptionsMonitor<Setting> intSettings,
        IJobStatusService jobStatusService,
        ISender sender,
        IHttpContextAccessor httpContextAccessor) : IRepositoryWrapper
    {

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
                _smChannel ??= new SMChannelsRepository(SMChannelLogger, this, repositoryContext, mapper, intSettings, iconService);
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
                _streamGroup ??= new StreamGroupRepository(StreamGroupRepositoryLogger, repositoryContext, mapper, intSettings, httpContextAccessor);
                return _streamGroup;
            }
        }

        private IChannelGroupRepository _channelGroup;

        public IChannelGroupRepository ChannelGroup
        {
            get
            {
                _channelGroup ??= new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, mapper, intSettings, sender);
                return _channelGroup;
            }
        }

        private IM3UFileRepository _m3uFile;

        public IM3UFileRepository M3UFile
        {
            get
            {
                _m3uFile ??= new M3UFileRepository(M3UFileRepositoryLogger, this, jobStatusService, repositoryContext, intSettings, mapper);
                return _m3uFile;
            }
        }

        private IVideoStreamLinkRepository _videoStreamLink;

        public IVideoStreamLinkRepository VideoStreamLink
        {
            get
            {
                _videoStreamLink ??= new VideoStreamLinkRepository(VideoStreamLinkRepositoryLogger, repositoryContext, intSettings, mapper);
                return _videoStreamLink;
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

        private IVideoStreamRepository _videoStream;

        public IVideoStreamRepository VideoStream
        {
            get
            {
                _videoStream ??= new VideoStreamRepository(VideoStreamRepositoryLogger, this, schedulesDirectDataService, iconService, repositoryContext, mapper, intSettings, sender);
                return _videoStream;
            }
        }


        private IStreamGroupVideoStreamRepository _streamGroupVideoStream;
        public IStreamGroupVideoStreamRepository StreamGroupVideoStream
        {
            get
            {
                _streamGroupVideoStream ??= new StreamGroupVideoStreamRepository(StreamGroupVideoStreamRepositoryLogger, repositoryContext, this, mapper, intSettings, sender);
                return _streamGroupVideoStream;
            }
        }

        private IStreamGroupChannelGroupRepository _streamGroupChannelGroup;
        public IStreamGroupChannelGroupRepository StreamGroupChannelGroup
        {
            get
            {
                _streamGroupChannelGroup ??= new StreamGroupChannelGroupRepository(StreamGroupChannelGroupRepositoryLogger, repositoryContext, this, mapper, intSettings, sender);
                return _streamGroupChannelGroup;
            }
        }



        public async Task<int> SaveAsync()
        {
            return await repositoryContext.SaveChangesAsync();
        }
    }
}