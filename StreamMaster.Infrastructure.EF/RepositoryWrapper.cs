using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

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
        ILogger<StreamGroupVideoStreamRepository> StreamGroupVideoStreamRepositoryLogger,
        ILogger<StreamGroupChannelGroupRepository> StreamGroupChannelGroupRepositoryLogger,
        ISchedulesDirectDataService schedulesDirectDataService,

        PGSQLRepositoryContext repositoryContext,
        //ISortHelper<StreamGroup> streamGroupSortHelper,
        IMapper mapper,
        IIconService iconService,
        IMemoryCache memoryCache,
        ISender sender,
        IHttpContextAccessor httpContextAccessor) : IRepositoryWrapper
    {
        private IStreamGroupRepository _streamGroup;

        public IStreamGroupRepository StreamGroup
        {
            get
            {
                _streamGroup ??= new StreamGroupRepository(StreamGroupRepositoryLogger, repositoryContext, mapper, memoryCache, httpContextAccessor);
                return _streamGroup;
            }
        }

        private IChannelGroupRepository _channelGroup;

        public IChannelGroupRepository ChannelGroup
        {
            get
            {
                _channelGroup ??= new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, sender);
                return _channelGroup;
            }
        }

        private IM3UFileRepository _m3uFile;

        public IM3UFileRepository M3UFile
        {
            get
            {
                _m3uFile ??= new M3UFileRepository(M3UFileRepositoryLogger, repositoryContext, mapper);
                return _m3uFile;
            }
        }

        private IVideoStreamLinkRepository _videoStreamLink;

        public IVideoStreamLinkRepository VideoStreamLink
        {
            get
            {
                _videoStreamLink ??= new VideoStreamLinkRepository(VideoStreamLinkRepositoryLogger, repositoryContext, mapper);
                return _videoStreamLink;
            }
        }

        private IEPGFileRepository _epgFile;

        public IEPGFileRepository EPGFile
        {
            get
            {
                _epgFile ??= new EPGFileRepository(EPGFileRepositoryLogger, repositoryContext, this, schedulesDirectDataService, mapper);
                return _epgFile;
            }
        }

        private IVideoStreamRepository _videoStream;

        public IVideoStreamRepository VideoStream
        {
            get
            {
                _videoStream ??= new VideoStreamRepository(VideoStreamRepositoryLogger, this, schedulesDirectDataService, iconService, repositoryContext, mapper, memoryCache, sender);
                return _videoStream;
            }
        }


        private IStreamGroupVideoStreamRepository _streamGroupVideoStream;
        public IStreamGroupVideoStreamRepository StreamGroupVideoStream
        {
            get
            {
                _streamGroupVideoStream ??= new StreamGroupVideoStreamRepository(StreamGroupVideoStreamRepositoryLogger, repositoryContext, this, mapper, sender);
                return _streamGroupVideoStream;
            }
        }

        private IStreamGroupChannelGroupRepository _streamGroupChannelGroup;
        public IStreamGroupChannelGroupRepository StreamGroupChannelGroup
        {
            get
            {
                _streamGroupChannelGroup ??= new StreamGroupChannelGroupRepository(StreamGroupChannelGroupRepositoryLogger, repositoryContext, this, mapper, sender);
                return _streamGroupChannelGroup;
            }
        }



        public async Task<int> SaveAsync()
        {
            return await repositoryContext.SaveChangesAsync();
        }
    }
}