using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

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
        private readonly ISortHelper<EPGFile> _epgFileSortHelper;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IMemoryCache _memoryCache;

        public RepositoryWrapper(RepositoryContext repositoryContext, ISortHelper<EPGFile> epgGFileSortHelper, ISortHelper<StreamGroup> streamGroupSortHelper, ISortHelper<M3UFile> m3uFileSortHelper, ISortHelper<VideoStream> videoStreamSortHelper, ISortHelper<ChannelGroup> channelGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender)
        {
            _repoContext = repositoryContext;
            _m3uFileSortHelper = m3uFileSortHelper;
            _videoStreamSortHelper = videoStreamSortHelper;
            _channelGroupSortHelper = channelGroupSortHelper;
            _streamGroupSortHelper = streamGroupSortHelper;
            _epgFileSortHelper = epgGFileSortHelper;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _sender = sender;
        }

        private IStreamGroupRepository _streamGroup;

        public IStreamGroupRepository StreamGroup
        {
            get
            {
                if (_streamGroup == null)
                {
                    _streamGroup = new StreamGroupRepository(_repoContext, _streamGroupSortHelper, _mapper, _memoryCache, _sender);
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
                    _channelGroup = new ChannelGroupRepository(_repoContext, _mapper, _sender);
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
                    _m3uFile = new M3UFileRepository(_repoContext, _m3uFileSortHelper);
                }
                return _m3uFile;
            }
        }

        private IEPGFileRepository _epgFile;

        public IEPGFileRepository EPGFile
        {
            get
            {
                if (_epgFile == null)
                {
                    _epgFile = new EPGFileRepository(_repoContext, _epgFileSortHelper);
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
                    _videoStream = new VideoStreamRepository(_repoContext, _mapper, _memoryCache, _sender);
                }
                return _videoStream;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _repoContext.SaveChangesAsync();
        }
    }
}