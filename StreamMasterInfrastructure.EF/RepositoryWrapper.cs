﻿using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

using StreamMasterInfrastructureEF.Repositories;

namespace StreamMasterInfrastructureEF
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly RepositoryContext _repoContext;
        private readonly ISortHelper<M3UFile> _m3uFileSortHelper;
        private readonly ISortHelper<VideoStream> _videoStreamSortHelper;
        private readonly ISortHelper<ChannelGroup> _channelGroupSortHelper;
        private readonly ISortHelper<StreamGroup> _streamGroupSortHelper;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChannelGroupRepository> _channelGroupRepository;
        private readonly ISettingsService _settingsService;
        public RepositoryWrapper(ILogger<ChannelGroupRepository> channelGroupRepository, RepositoryContext repositoryContext, ISortHelper<StreamGroup> streamGroupSortHelper, ISortHelper<M3UFile> m3uFileSortHelper, ISortHelper<VideoStream> videoStreamSortHelper, ISortHelper<ChannelGroup> channelGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender, IHttpContextAccessor httpContextAccessor, ISettingsService settingsService)
        {
            _repoContext = repositoryContext;
            _m3uFileSortHelper = m3uFileSortHelper;
            _videoStreamSortHelper = videoStreamSortHelper;
            _channelGroupSortHelper = channelGroupSortHelper;
            _streamGroupSortHelper = streamGroupSortHelper;
            _httpContextAccessor = httpContextAccessor;
            _settingsService = settingsService;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _sender = sender;
            _channelGroupRepository = channelGroupRepository;
        }

        private IStreamGroupRepository _streamGroup;

        public IStreamGroupRepository StreamGroup
        {
            get
            {
                if (_streamGroup == null)
                {
                    _streamGroup = new StreamGroupRepository(_repoContext, _streamGroupSortHelper, _mapper, _memoryCache, _sender, _httpContextAccessor, _settingsService);
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
                    _channelGroup = new ChannelGroupRepository(_channelGroupRepository, _repoContext, _mapper, _memoryCache, _sender);
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
                    _m3uFile = new M3UFileRepository(_repoContext, _mapper);
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
                    _videoStreamLink = new VideoStreamLinkRepository(_repoContext, _mapper, _memoryCache, _sender);
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
                    _epgFile = new EPGFileRepository(_repoContext, _mapper);
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
                    _videoStream = new VideoStreamRepository(_repoContext, _mapper, _memoryCache, _sender, _settingsService);
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
                    _streamGroupVideoStream = new StreamGroupVideoStreamRepository(_repoContext, this, _mapper, _settingsService, _sender);
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
                    _streamGroupChannelGroup = new StreamGroupChannelGroupRepository(_repoContext, this, _mapper, _settingsService, _sender);
                }
                return _streamGroupChannelGroup;
            }
        }



        public async Task<int> SaveAsync()
        {
            return await _repoContext.SaveChangesAsync();
        }
    }
}