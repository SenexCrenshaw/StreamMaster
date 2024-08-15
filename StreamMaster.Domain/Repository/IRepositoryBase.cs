using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

/// <summary>
/// Generic repository interface that provides CRUD operations along with some bulk and specific query capabilities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T> where T : class
{
    //Setting Settings { get; }
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default);
    T? FirstOrDefault(Expression<Func<T, bool>> expression, bool tracking = false);

    bool Any(Expression<Func<T, bool>> expression);
    IQueryable<T> GetQuery(bool tracking = false);
    IQueryable<T> GetQuery(Expression<Func<T, bool>> expression, bool tracking = false);
    /// <summary>
    /// Retrieves entities based on the provided query parameters.
    /// </summary>
    /// <param name="parameters">The parameters for the query.</param>
    /// <returns>An IQueryable of entities.</returns>
    IQueryable<T> GetQuery(QueryStringParameters parameters, bool tracking = false);
    /// <summary>
    /// Counts the total number of entities.
    /// </summary>
    /// <returns>The total count.</returns>
    int Count();

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    void Create(T entity);

    /// <summary>
    /// Adds a range of entities to the database.
    /// </summary>
    /// <param name="entities">Entities to add.</param>
    void CreateRange(T[] entities);
    void CreateRange(List<T> entities);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Updates a range of entities in the database.
    /// </summary>
    /// <param name="entities">Entities to update.</param>
    void UpdateRange(T[] entities);

    /// <summary>
    /// Removes an entity from the database.
    /// </summary>
    /// <param name="entity">Entity to remove.</param>
    void Delete(T entity);


    Task BulkDeleteAsync(IQueryable<T> query);


    /// <summary>
    /// Performs a bulk insert operation.
    /// </summary>
    /// <param name="entities">Entities to insert.</param>
    void BulkInsert(T[] entities);

    void BulkInsert(List<T> entities);
    Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    /// <summary>
    /// Performs a bulk update operation.
    /// </summary>
    /// <param name="entities">Entities to update.</param>
    void BulkUpdate(T[] entities);

    void BulkUpdate(List<T> entities);
    Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    /// <summary>
    /// Performs a bulk delete operation based on the provided query.
    /// </summary>
    /// <param name="query">The query determining which entities to delete.</param>
    //void BulkDelete(IQueryable<T> query);
    Task<int> SaveChangesAsync();
}