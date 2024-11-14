using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Filtering;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Provides base functionalities for repositories.
/// </summary>
/// <typeparam name="T">Type of the entity managed by this repository.</typeparam>
public abstract class RepositoryBase<T>(IRepositoryContext RepositoryContext, ILogger intLogger) : IRepositoryBase<T> where T : class
{
    internal readonly IRepositoryContext RepositoryContext = RepositoryContext;
    internal readonly ILogger logger = intLogger;

    #region Get 
    public virtual IQueryable<T> GetQuery(bool tracking = false)
    {
        return tracking ? RepositoryContext.Set<T>() : RepositoryContext.Set<T>().AsNoTracking();
    }

    public virtual IQueryable<T> GetQuery(QueryStringParameters parameters, bool tracking = false)
    {
        // If there are no filters or order specified, just return all entities.
        if (string.IsNullOrEmpty(parameters.JSONFiltersString) && string.IsNullOrEmpty(parameters.OrderBy))
        {
            return GetQuery(tracking);
        }

        List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(parameters.JSONFiltersString);
        return GetQuery(filters, parameters.OrderBy, tracking: tracking);
    }

    public IQueryable<T> GetQuery(List<DataTableFilterMetaData>? filters, string orderBy, IQueryable<T>? entities = null, bool tracking = false)
    {
        entities ??= GetQuery(tracking);
        return FilterHelper<T>.ApplyFiltersAndSort(entities, filters, orderBy);
    }

    public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> expression, bool tracking = false)
    {
        return GetQuery(tracking).Where(expression);
    }

    public IQueryable<T> GetQuery(Expression<Func<T, bool>> expression, string orderBy)
    {
        if (expression == null)
        {
            logger.LogWarning("The filtering expression provided to GetQuery is null.");
            throw new ArgumentNullException(nameof(expression));
        }

        if (string.IsNullOrWhiteSpace(orderBy))
        {
            logger.LogWarning("The orderBy parameter provided to GetQuery is null or empty.");
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
            logger.LogError(ex, "Error occurred while trying to fetch and order entities based on condition and order '{orderBy}'.", orderBy);
            throw;
        }
    }

    public bool Any(Expression<Func<T, bool>> expression)
    {
        return RepositoryContext.Set<T>().Any(expression);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = false, CancellationToken cancellationToken = default)
    {
        return await GetQuery(tracking).FirstOrDefaultAsync(expression, cancellationToken);
    }

    public T? FirstOrDefault(Expression<Func<T, bool>> expression, bool tracking = false)
    {
        return FirstOrDefaultAsync(expression, tracking: tracking).Result;
    }

    #endregion

    #region Util
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return 0;
        }
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

    #endregion

    #region CRUD
    // ... [Other methods, following similar patterns]

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">Entity to be created.</param>
    public void Create(T entity, bool? track = true)
    {
        if (entity == null)
        {
            logger.LogWarning("Attempted to add a null entity.");
            throw new ArgumentNullException(nameof(entity));
        }

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entry = RepositoryContext.Set<T>().Add(entity);
        if (track == false)
        {
            entry.State = EntityState.Detached;
        }
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
        if (entities == null || entities.Length == 0)
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
    //public void BulkInsert(List<T> entities)
    //{
    //    if (entities == null || entities.Count == 0)
    //    {
    //        logger.LogWarning("Attempted to perform a bulk insert with null or empty entities.");
    //        throw new ArgumentNullException(nameof(entities));
    //    }

    //    RepositoryContext.BulkInsertEntities(entities);
    //}

    /// <summary>
    /// Performs a bulk insert of entities.
    /// </summary>
    /// <param name="entities">Entities to be inserted.</param>
    //public void BulkInsert(T[] entities)
    //{
    //    if (entities == null || entities.Length == 0)
    //    {
    //        logger.LogWarning("Attempted to perform a bulk insert with null or empty entities.");
    //        throw new ArgumentNullException(nameof(entities));
    //    }

    //    RepositoryContext.BulkInsertEntities(entities);
    //}

    /// <summary>
    /// Deletes a group of entities based on a query.
    /// </summary>
    /// <param name="query">The IQueryable to select entities to be deleted.</param>
    //public void BulkDelete(IQueryable<T> query)
    //{
    //    if (query?.Any() != true)
    //    {
    //        logger.LogWarning("Attempted to perform a bulk delete with a null or empty query.");
    //        throw new ArgumentNullException(nameof(query));
    //    }
    //    DbContext? context = RepositoryContext as DbContext;

    //    context!.BulkDelete(query);
    //}

    public async Task BulkDeleteAsync<TEntity>(List<TEntity> items, int batchSize = 100, CancellationToken cancellationToken = default) where TEntity : class
    {
        if (items == null || items.Count == 0)
        {
            logger.LogWarning("Attempted to perform a bulk delete with a null or empty list of items.");
            throw new ArgumentNullException(nameof(items));
        }

        for (int i = 0; i < items.Count; i += batchSize)
        {
            List<TEntity> batch = items.Skip(i).Take(batchSize).ToList();

            // Attach each item and mark for removal
            foreach (TEntity? item in batch)
            {
                RepositoryContext.Set<TEntity>().Attach(item);
                RepositoryContext.Set<TEntity>().Remove(item);
            }

            await RepositoryContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Deletes a group of entities based on a query.
    /// </summary>
    /// <param name="query">The IQueryable to select entities to be deleted.</param>
    public async Task BulkDeleteAsync(IQueryable<T> query)
    {
        if (query?.Any() != true)
        {
            logger.LogWarning("Attempted to perform a bulk delete with a null or empty query.");
            throw new ArgumentNullException(nameof(query));
        }

        await RepositoryContext.BulkDeleteAsyncEntities(query);
    }

    ///// <summary>
    ///// Performs a bulk update on a set of entities.
    ///// </summary>
    ///// <param name="entities">Entities to be updated.</param>
    //public void BulkUpdate(T[] entities)
    //{
    //    if (entities == null || entities.Length == 0)
    //    {
    //        logger.LogWarning("Attempted to perform a bulk update with null or empty entities.");
    //        throw new ArgumentNullException(nameof(entities));
    //    }

    //    RepositoryContext.BulkUpdateEntities(entities);
    //}

    public void BulkUpdate(List<T> entities)
    {
        if (entities == null || entities.Count == 0)
        {
            logger.LogWarning("Attempted to perform a bulk update with null or empty entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.BulkUpdateEntitiesAsync(entities);
    }

    /// <summary>
    /// Inserts a range of entities.
    /// </summary>
    /// <param name="entities">Entities to be inserted.</param>
    public void CreateRange(T[] entities)
    {
        if (entities == null || entities.Length == 0)
        {
            logger.LogWarning("Attempted to insert a null or empty array of entities.");
            throw new ArgumentNullException(nameof(entities));
        }

        RepositoryContext.Set<T>().AddRange(entities);
    }

    public void CreateRange(List<T> entities)
    {
        if (entities == null || entities.Count == 0)
        {
            logger.LogWarning("Attempted to insert a null or empty array of entities.");
            throw new ArgumentNullException(nameof(entities));
        }
        RepositoryContext.Set<T>().AddRange(entities);
    }

    //public async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    //{
    //    await RepositoryContext.BulkInsertEntitiesAsync(entities);
    //}

    public async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        await RepositoryContext.BulkUpdateAsync(entities);
    }

    #endregion
}