using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Repository;

using System.Linq.Expressions;

namespace StreamMasterInfrastructure.EF
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext { get; set; }
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {
            return RepositoryContext.Set<T>().AsNoTracking();
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return RepositoryContext.Set<T>()
                .Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }
        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }
        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public void Create(T[] entities)
        {
            RepositoryContext.Set<T>().AddRange(entities);
        }
    }
}
