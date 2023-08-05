using System;
using System.Linq;
using System.Linq.Expressions;

namespace StreamMasterDomain.Repository
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Create(T[] entities);
        void Update(T entity);
        void Delete(T entity);
    }
}
