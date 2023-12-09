using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

using StreamMasterInfrastructureEF.Repositories;

namespace StreamMasterInfrastructureEF
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
        ISchedulesDirectData schedulesDirectData,
        RepositoryContext repositoryContext,
        ISortHelper<StreamGroup> streamGroupSortHelper,
        IMapper mapper, IMemoryCache memoryCache, ISender sender,
        IHttpContextAccessor httpContextAccessor, ISettingsService settingsService) : IRepositoryWrapper
    {
        private IStreamGroupRepository _streamGroup;

        public IStreamGroupRepository StreamGroup
        {
            get
            {
                if (_streamGroup == null)
                {
                    _streamGroup = new StreamGroupRepository(StreamGroupRepositoryLogger, repositoryContext, this, streamGroupSortHelper, mapper, memoryCache, sender, httpContextAccessor, settingsService);
                }
                return _streamGroup;
            }
        }

        private IChannelGroupRepository _channelGroup;

        public IChannelGroupRepository ChannelGroup
        {
            get
            {
                if (_channelGroup == null)
                {
                    _channelGroup = new ChannelGroupRepository(ChannelGroupRepositoryLogger, repositoryContext, this, memoryCache, sender);
                }
                return _channelGroup;
            }
        }

        private IM3UFileRepository _m3uFile;

        public IM3UFileRepository M3UFile
        {
            get
            {
                if (_m3uFile == null)
                {
                    _m3uFile = new M3UFileRepository(M3UFileRepositoryLogger, repositoryContext, this, mapper);
                }
                return _m3uFile;
            }
        }

        private IVideoStreamLinkRepository _videoStreamLink;

        public IVideoStreamLinkRepository VideoStreamLink
        {
            get
            {
                if (_videoStreamLink == null)
                {
                    _videoStreamLink = new VideoStreamLinkRepository(VideoStreamLinkRepositoryLogger, repositoryContext, mapper, memoryCache, sender);
                }
                return _videoStreamLink;
            }
        }

        private IEPGFileRepository _epgFile;

        public IEPGFileRepository EPGFile
        {
            get
            {
                if (_epgFile == null)
                {
                    _epgFile = new EPGFileRepository(EPGFileRepositoryLogger, repositoryContext, this, schedulesDirectData, mapper);
                }
                return _epgFile;
            }
        }

        private IVideoStreamRepository _videoStream;

        public IVideoStreamRepository VideoStream
        {
            get
            {
                if (_videoStream == null)
                {
                    _videoStream = new VideoStreamRepository(VideoStreamRepositoryLogger, repositoryContext, mapper, memoryCache, sender, settingsService);
                }
                return _videoStream;
            }
        }


        private IStreamGroupVideoStreamRepository _streamGroupVideoStream;
        public IStreamGroupVideoStreamRepository StreamGroupVideoStream
        {
            get
            {
                if (_streamGroupVideoStream == null)
                {
                    _streamGroupVideoStream = new StreamGroupVideoStreamRepository(StreamGroupVideoStreamRepositoryLogger, repositoryContext, this, mapper, settingsService, sender);
                }
                return _streamGroupVideoStream;
            }
        }

        private IStreamGroupChannelGroupRepository _streamGroupChannelGroup;
        public IStreamGroupChannelGroupRepository StreamGroupChannelGroup
        {
            get
            {
                if (_streamGroupChannelGroup == null)
                {
                    _streamGroupChannelGroup = new StreamGroupChannelGroupRepository(StreamGroupChannelGroupRepositoryLogger, repositoryContext, this, mapper, settingsService, sender);
                }
                return _streamGroupChannelGroup;
            }
        }



        public async Task<int> SaveAsync()
        {
            return await repositoryContext.SaveChangesAsync();
        }
    }
}