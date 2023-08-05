using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructure.EF
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private ISortHelper<M3UFile> _m3uFileSortHelper;

        public RepositoryWrapper(RepositoryContext repositoryContext, ISortHelper<M3UFile> m3uFileSortHelper)
        {
            _repoContext = repositoryContext;
            _m3uFileSortHelper = m3uFileSortHelper;

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


        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
