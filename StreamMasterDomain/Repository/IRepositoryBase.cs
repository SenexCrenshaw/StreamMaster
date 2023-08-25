using StreamMasterDomain.Pagination;

using System.Linq.Expressions;

namespace StreamMasterDomain.Repository
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> GetIQueryableForEntity(QueryStringParameters parameters);
        int Count();
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Create(T[] entities);
        void Update(T entity);
        void UpdateRange(T[] entities);
        void Delete(T entity);
    }
}
