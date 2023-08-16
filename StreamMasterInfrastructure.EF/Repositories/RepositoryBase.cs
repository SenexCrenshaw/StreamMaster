using AutoMapper;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace StreamMasterInfrastructureEF.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private static readonly ConcurrentDictionary<Type, ParameterExpression> ParameterCache = new();
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> PropertyCache = new();

    public IQueryable<T> ApplyFiltersAndSort(IQueryable<T> query, List<DataTableFilterMetaData>? filters, string orderBy)
    {
        if (filters != null)
        {
            // Apply filters
            foreach (DataTableFilterMetaData filter in filters)
            {
                query = ApplyFilter(query, filter);
            }
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = query.OrderBy(orderBy);
        }

        return query;
    }

    private IQueryable<T> ApplyFilter(IQueryable<T> query, DataTableFilterMetaData filter)
    {
        if (!ParameterCache.TryGetValue(typeof(T), out ParameterExpression? parameter))
        {
            parameter = Expression.Parameter(typeof(T), "entity");
            ParameterCache[typeof(T)] = parameter;
        }

        (Type, string FieldName) propertyKey = (typeof(T), filter.FieldName);
        if (!PropertyCache.TryGetValue(propertyKey, out PropertyInfo? property))
        {
            property = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, filter.FieldName, StringComparison.OrdinalIgnoreCase));
            if (property != null)
            {
                PropertyCache[propertyKey] = property;
            }
            else
            {
                throw new ArgumentException($"Property {filter.FieldName} not found on type {typeof(T).FullName}");
            }
        }

        Expression propertyAccess = Expression.Property(parameter, property);
        Expression filterExpression;

        switch (filter.MatchMode)
        {
            case "channelGroups":
                filterExpression = CreateChannelGroupsExpression(filter, propertyAccess);
                break;
            case "contains":
            case "startsWith":
            case "endsWith":
                filterExpression = CreateStringMatchExpression(filter, propertyAccess, filter.MatchMode);
                break;
            case "equals":
                filterExpression = Expression.Equal(propertyAccess, Expression.Constant(ConvertValue(filter.Value, property.PropertyType)));
                break;
            default:
                filterExpression = Expression.Equal(propertyAccess, Expression.Constant(ConvertValue(filter.Value, property.PropertyType)));
                break;
        }

        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
        return query.Where(lambda);
    }

    private Expression CreateStringMatchExpression(DataTableFilterMetaData filter, Expression propertyAccess, string matchMode)
    {
        //MethodCallExpression toLowerCall = Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
        //string methodName = matchMode;  // "Contains", "StartsWith", or "EndsWith"

        //MethodInfo stringMethod = typeof(string).GetMethod(methodName, new[] { typeof(string) });
        //if (stringMethod == null)
        //{
        //    throw new InvalidOperationException($"Method {methodName} not found on string type.");
        //}
        return Expression.Call(
                        Expression.Call(propertyAccess, "ToLower", null),   // Convert property value to lowercase
                        matchMode, null, Expression.Constant(ConvertValue(filter.Value, typeof(string)).ToString().ToLower())  // Convert filter value to lowercase
                    );

    }

    private Expression CreateChannelGroupsExpression(DataTableFilterMetaData filter, Expression propertyAccess)
    {
        MethodCallExpression toLowerCall = Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));

        string[] channelGroups = JsonSerializer.Deserialize<string[]>(filter.Value.ToString());
        List<Expression> containsExpressions = new();
        foreach (string group in channelGroups)
        {
            MethodCallExpression containsCall = Expression.Call(toLowerCall, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(group.Trim().ToLower()));
            containsExpressions.Add(containsCall);
        }

        Expression filterExpression = containsExpressions[0];
        for (int i = 1; i < containsExpressions.Count; i++)
        {
            filterExpression = Expression.OrElse(filterExpression, containsExpressions[i]);
        }

        return filterExpression;
    }

    private object ConvertValue(object value, Type targetType)
    {
        // Handle null values immediately
        if (value == null)
        {
            return null;
        }


        // If targetType is nullable, get the underlying type
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        if (targetType == typeof(string))
        {
            return value.ToString();
        }

        if (targetType == typeof(bool))
        {
            return bool.TryParse(value.ToString(), out bool parsedValue) && parsedValue;
        }

        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value.ToString());
        }

        // For all other types, attempt to change the type
        return Convert.ChangeType(value, targetType);
    }


    protected RepositoryContext RepositoryContext { get; set; }

    public RepositoryBase(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }

    public IQueryable<T> FindAll()
    {
        return RepositoryContext.Set<T>().AsNoTracking();
    }

    public async Task<PagedResponse<TDto>> GetEntitiesAsync<TDto>(QueryStringParameters parameters, IMapper mapper)
where TDto : class
    {
        IQueryable<T> entities;

        if (!string.IsNullOrEmpty(parameters.JSONFiltersString) || !string.IsNullOrEmpty(parameters.OrderBy))
        {
            List<DataTableFilterMetaData>? filters = null;
            if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
            {
                filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(parameters.JSONFiltersString);
                entities = FindByCondition(filters, parameters.OrderBy);
            }
            else
            {
                entities = FindAll();
            }
        }
        else
        {
            entities = FindAll();
        }

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

    public IQueryable<T> FindByCondition(List<DataTableFilterMetaData>? filters, string orderBy)
    {
        DbSet<T> query = RepositoryContext.Set<T>();

        // Apply filters and sorting
        IQueryable<T> filteredAndSortedQuery = ApplyFiltersAndSort(query, filters, orderBy);

        return filteredAndSortedQuery.AsNoTracking();
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