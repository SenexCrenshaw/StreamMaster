using EFCore.BulkExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Filtering;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Provides base functionalities for repositories.
/// </summary>
/// <typeparam name="T">Type of the entity managed by this repository.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly IRepositoryContext RepositoryContext;
    protected readonly ILogger logger;

    public RepositoryBase(IRepositoryContext repositoryContext, ILogger logger)
    {
        RepositoryContext = repositoryContext ?? throw new ArgumentNullException(nameof(repositoryContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> SaveChangesAsync()
    {

        try
        {
            return await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // You can decide how to handle exceptions here, for example by
            // logging them. In this case, we're simply swallowing the exception.
        }
        return 0;

    }

    /// <summary>
    /// Count the number of entities in the database.
    /// </summary>
    /// <returns>The number of entities.</returns>
    public int Count()
    {
        try
        {
            return RepositoryContext.Set<T>().AsNoTracking().Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while counting entities.");
            throw;
        }
    }

    /// <summary>
    /// Retrieve all entities.
    /// </summary>
    /// <returns>IQueryable of all entities.</returns>
    internal IQueryable<T> FindAll()
    {
        return RepositoryContext.Set<T>().AsNoTracking();
    }

    /// <summary>
    /// Retrieve all entities.
    /// </summary>
    /// <returns>IQueryable of all entities.</returns>
    internal IQueryable<T> FindAllWithTracking()
    {
        return RepositoryContext.Set<T>();
    }

    /// <summary>
    /// Retrieve entities based on given parameters.
    /// </summary>
    /// <param name="parameters">Query parameters.</param>
    /// <returns>IQueryable of entities that satisfy conditions from parameters.</returns>
    public IQueryable<T> GetIQueryableForEntity(QueryStringParameters parameters)
    {
        // If there are no filters or order specified, just return all entities.
        if (string.IsNullOrEmpty(parameters.JSONFiltersString) && string.IsNullOrEmpty(parameters.OrderBy))
        {
            return FindAll();
        }

        List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(parameters.JSONFiltersString);
        return FindByCondition(filters, parameters.OrderBy);
    }

    /// <summary>
    /// Filters and sorts entities based on conditions.
    /// </summary>
    /// <param name="filters">List of filters.</param>
    /// <param name="orderBy">Ordering condition.</param>
    /// <param name="entities">Optional: IQueryable of entities to start with.</param>
    /// <returns>IQueryable of filtered and sorted entities.</returns>
    public IQueryable<T> FindByCondition(List<DataTableFilterMetaData>? filters, string orderBy, IQueryable<T>? entities = null)
    {
        entities ??= RepositoryContext.Set<T>().AsNoTracking();
        return FilterHelper<T>.ApplyFiltersAndSort(entities, filters, orderBy);
    }

    /// <summary>
    /// Retrieves entities that satisfy the given condition.
    /// </summary>
    /// <param name="expression">Condition to be checked.</param>
    /// <returns>IQueryable of entities that satisfy the condition.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return RepositoryContext.Set<T>().Where(expression).AsNoTracking();
    }

    public IQueryable<T> FindByConditionTracked(Expression<Func<T, bool>> expression)
    {
        return RepositoryContext.Set<T>().Where(expression);
    }

    // ... [Other methods, following similar patterns]

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">Entity to be created.</param>
    public void Create(T entity)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to add a null entity.");
            throw new ArgumentNullException(nameof(entity));
        }

        RepositoryContext.Set<T>().Add(entity);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity to be updated.</param>
    public void Update(T entity)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to update a null entity.");
            throw new ArgumentNullException(nameof(entity));
        }

        RepositoryContext.Set<T>().Update(entity);
    }

    /// <summary>
    /// Updates a range of entities.
    /// </summary>
    /// <param name="entities">Entities to be updated.</param>
    public void UpdateRange(T[] entities)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to update a null or empty array of entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.Set<T>().UpdateRange(entities);
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to be deleted.</param>
    public void Delete(T entity)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to delete a null entity.");
            throw new ArgumentNullException(nameof(entity));
        }

        RepositoryContext.Set<T>().Remove(entity);
    }

    /// <summary>
    /// Performs a bulk insert of entities.
    /// </summary>
    /// <param name="entities">Entities to be inserted.</param>
    public void BulkInsert(List<T> entities)
    {
        if (entities == null || entities.Count == 0)
        {
            logger.LogWarning("Attempted to perform a bulk insert with null or empty entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.BulkInsertEntities(entities);
    }

    /// <summary>
    /// Performs a bulk insert of entities.
    /// </summary>
    /// <param name="entities">Entities to be inserted.</param>
    public void BulkInsert(T[] entities)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to perform a bulk insert with null or empty entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.BulkInsertEntities(entities);
    }

    /// <summary>
    /// Deletes a group of entities based on a query.
    /// </summary>
    /// <param name="query">The IQueryable to select entities to be deleted.</param>
    public void BulkDelete(IQueryable<T> query)
    {
        if (query == null || !query.Any())
        {
            logger.LogWarning("Attempted to perform a bulk delete with a null or empty query.");
            throw new ArgumentNullException(nameof(query));
        }
        DbContext context = RepositoryContext as DbContext;

        context.BulkDelete(query);
    }

    /// <summary>
    /// Deletes a group of entities based on a query.
    /// </summary>
    /// <param name="query">The IQueryable to select entities to be deleted.</param>
    public async Task BulkDeleteAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        if (query == null || !query.Any())
        {
            logger.LogWarning("Attempted to perform a bulk delete with a null or empty query.");
            throw new ArgumentNullException(nameof(query));
        }

        await RepositoryContext.BulkDeleteAsyncEntities(query, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Performs a bulk update on a set of entities.
    /// </summary>
    /// <param name="entities">Entities to be updated.</param>
    public void BulkUpdate(T[] entities)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to perform a bulk update with null or empty entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.BulkUpdateEntities(entities);
    }

    public void BulkUpdate(List<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to perform a bulk update with null or empty entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.BulkUpdateEntities(entities);
    }

    /// <summary>
    /// Inserts a range of entities.
    /// </summary>
    /// <param name="entities">Entities to be inserted.</param>
    public void CreateRange(T[] entities)
    {
        if (entities == null || !entities.Any())
        {
            logger.LogWarning("Attempted to insert a null or empty array of entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.Set<T>().AddRange(entities);
    }

    /// <summary>
    /// Retrieves entities that match the provided condition and orders them based on the provided string.
    /// </summary>
    /// <param name="expression">The filtering condition.</param>
    /// <param name="orderBy">The property by which to order the entities.</param>
    /// <returns>An IQueryable of entities.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, string orderBy)
    {
        if (expression == null)
        {
            logger.LogWarning("The filtering expression provided to FindByCondition is null.");
            throw new ArgumentNullException(nameof(expression));
        }

        if (string.IsNullOrWhiteSpace(orderBy))
        {
            logger.LogWarning("The orderBy parameter provided to FindByCondition is null or empty.");
            throw new ArgumentException("Ordering parameter must not be null or empty.", nameof(orderBy));
        }

        try
        {
            IQueryable<T> test = RepositoryContext.Set<T>().Where(expression).AsNoTracking();
            if (!orderBy.Contains(','))
            {
                return test.OrderBy(orderBy);
            }
            string[] orderByParts = orderBy.Trim().Split(',');
            foreach (string orderByPart in orderByParts)
            {
                test = test.OrderBy(orderByPart);
            }

            return test;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while trying to fetch and order entities based on condition and order '{orderBy}'.");
            throw;
        }
    }
}