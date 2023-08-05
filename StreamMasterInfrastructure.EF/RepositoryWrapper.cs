using AutoMapper;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructure.EF
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private ISortHelper<M3UFile> _m3uFileSortHelper;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        ISortHelper<VideoStream> _videoStreamSortHelper;
        public RepositoryWrapper(RepositoryContext repositoryContext, ISortHelper<M3UFile> m3uFileSortHelper, ISortHelper<VideoStream> videoStreamSortHelper, IMapper mapper, IMemoryCache memoryCache)
        {
            _repoContext = repositoryContext;
            _m3uFileSortHelper = m3uFileSortHelper;
            _videoStreamSortHelper = videoStreamSortHelper;
            _mapper = mapper;
            _memoryCache = memoryCache;
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


        private IVideoStreamRepository _videoStream;

        public IVideoStreamRepository VideoStream
        {
            get
            {
                if (_videoStream == null)
                {
                    _videoStream = new VideoStreamRepository(_repoContext, _videoStreamSortHelper, _mapper, _memoryCache);
                }
                return _videoStream;

            }
        }


        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
