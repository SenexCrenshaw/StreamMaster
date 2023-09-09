using AutoMapper;

using EFCore.BulkExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Common;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;


namespace StreamMasterInfrastructureEF.Repositories;
public abstract class RepositoryBase<T, TDto>(RepositoryContext repositoryContext) : IRepositoryBase<T, TDto> where T : class where TDto : new()
{
    protected RepositoryContext RepositoryContext { get; set; } = repositoryContext;

    public PagedResponse<TDto> CreateEmptyPagedResponse(QueryStringParameters parameters)
    {
        return parameters.CreateEmptyPagedResponse<TDto>(this.Count());
    }

    public int Count()
    {
        return RepositoryContext.Set<T>().AsNoTracking().Count();
    }
    public IQueryable<T> FindAll()
    {
        return RepositoryContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> GetIQueryableForEntity(QueryStringParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.JSONFiltersString) || !string.IsNullOrEmpty(parameters.OrderBy))
        {
            if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
            {
                List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(parameters.JSONFiltersString);
                return FindByCondition(filters, parameters.OrderBy);
            }
        }

        return FindAll();

    }

    public async Task<PagedResponse<TDto>> GetEntitiesAsync(QueryStringParameters parameters, IMapper mapper)
    {
        IQueryable<T> entities = GetIQueryableForEntity(parameters);

        IPagedList<T> pagedResult = await entities.ToPagedListAsync(parameters.PageNumber, parameters.PageSize).ConfigureAwait(false);

        // If there are no entities, return an empty response early
        if (!pagedResult.Any())
        {
            return new PagedResponse<TDto>
            {
                PageNumber = parameters.PageNumber,
                TotalPageCount = 0,
                PageSize = parameters.PageSize,
                TotalItemCount = 0,
                Data = new List<TDto>()
            };
        }

        List<TDto> destination = mapper.Map<List<TDto>>(pagedResult);

        StaticPagedList<TDto> test = new(destination, pagedResult.GetMetaData());

        // Use the TotalItemCount from the metadata instead of counting entities again
        int totalCount = pagedResult.TotalItemCount;
        PagedResponse<TDto> pagedResponse = test.ToPagedResponse(totalCount);

        return pagedResponse;
    }


    public IQueryable<T> FindByConditionWithJSONFilter(string JSONFiltersString, string orderBy, IQueryable<T>? entities = null)
    {
        List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(JSONFiltersString);
        return FindByCondition(filters, orderBy, entities);
    }
    public IQueryable<T> FindByCondition(List<DataTableFilterMetaData>? filters, string orderBy, IQueryable<T>? entities = null)
    {
        if (entities == null)
        {
            entities = RepositoryContext.Set<T>();
        }

        // Apply filters and sorting
        IQueryable<T> filteredAndSortedQuery = FilterHelper<T>.ApplyFiltersAndSort(entities, filters, orderBy);

        return filteredAndSortedQuery;//.AsNoTracking();
    }


    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return RepositoryContext.Set<T>()
            .Where(expression).AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, string orderBy)
    {
        return RepositoryContext.Set<T>()
            .Where(expression).OrderBy(orderBy).AsNoTracking();
    }


    public void Create(T entity)
    {
        RepositoryContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        RepositoryContext.Set<T>().Update(entity);
    }

    public void UpdateRange(T[] entities)
    {
        RepositoryContext.Set<T>().UpdateRange(entities);
    }


    public void Delete(T entity)
    {
        RepositoryContext.Set<T>().Remove(entity);
    }

    public void BulkInsert(List<T> entities)
    {
        RepositoryContext.BulkInsert(entities);
    }
    public void BulkInsert(T[] entities)
    {

        RepositoryContext.BulkInsert(entities, options =>
        {
            options.PropertiesToIncludeOnUpdate = new List<string> { "" };
        });
        RepositoryContext.SaveChanges();
    }
    public void BulkDelete(IQueryable<T> query)
    {
        RepositoryContext.BulkDelete(query);
    }
    public void BulkUpdate(T[] entities)
    {
        RepositoryContext.BulkUpdate(entities);
    }

    public void Create(T[] entities)
    {
        RepositoryContext.Set<T>().AddRange(entities);
    }


}