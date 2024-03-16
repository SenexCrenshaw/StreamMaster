using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

/// <summary>
/// Generic repository interface that provides CRUD operations along with some bulk and specific query capabilities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T> where T : class
{
    /// <summary>
    /// Performs a bulk delete operation based on the provided query.
    /// </summary>
    /// <param name="query">The query determining which entities to delete.</param>
    void BulkDelete(IQueryable<T> query);
    Task<int> SaveChangesAsync();
    Task BulkDeleteAsync(IQueryable<T> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk insert operation.
    /// </summary>
    /// <param name="entities">Entities to insert.</param>
    void BulkInsert(T[] entities);

    void BulkInsert(List<T> entities);

    /// <summary>
    /// Performs a bulk update operation.
    /// </summary>
    /// <param name="entities">Entities to update.</param>
    void BulkUpdate(T[] entities);

    void BulkUpdate(List<T> entities);

    /// <summary>
    /// Retrieves entities based on the provided query parameters.
    /// </summary>
    /// <param name="parameters">The parameters for the query.</param>
    /// <returns>An IQueryable of entities.</returns>
    IQueryable<T> GetIQueryableForEntity(QueryStringParameters parameters);

    /// <summary>
    /// Retrieves entities that match the provided condition and orders them.
    /// </summary>
    /// <param name="expression">The filtering condition.</param>
    /// <param name="orderBy">The property by which to order the entities.</param>
    /// <returns>An IQueryable of entities.</returns>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, string orderBy);

    /// <summary>
    /// Retrieves entities that satisfy the given condition.
    /// </summary>
    /// <param name="expression">Condition to be checked.</param>
    /// <returns>IQueryable of entities that satisfy the condition.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    public IQueryable<T> FindByConditionTracked(Expression<Func<T, bool>> expression);
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
}