using AutoMapper;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.Json;

namespace StreamMasterInfrastructureEF.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        public IQueryable<T> ApplyFiltersAndSort(IQueryable<T> query, List<DataTableFilterMetaData>? filters, string orderBy)
        {
            if (filters != null)
            {
                // Apply filters
                foreach (var filter in filters)
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
            var parameter = Expression.Parameter(typeof(T), "entity");

            var property = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, filter.FieldName, StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                throw new ArgumentException($"Property {filter.FieldName} not found on type {typeof(T).FullName}");
            }

            Expression propertyAccess = Expression.Property(parameter, property);

            // Convert filter value to property type
            var convertedValue = ConvertValue(filter.Value, property.PropertyType);

            Expression filterExpression = null;

            switch (filter.MatchMode)
            {
                case "contains":
                    filterExpression = Expression.Call(
                        Expression.Call(propertyAccess, "ToLower", null),   // Convert property value to lowercase
                        "Contains", null, Expression.Constant(convertedValue.ToString().ToLower())  // Convert filter value to lowercase
                    );
                    break;

                case "startsWith":
                    filterExpression = Expression.Call(
                        Expression.Call(propertyAccess, "ToLower", null),   // Convert property value to lowercase
                        "StartsWith", null, Expression.Constant(convertedValue.ToString().ToLower())  // Convert filter value to lowercase
                    );
                    break;

                case "endsWith":
                    filterExpression = Expression.Call(
                        Expression.Call(propertyAccess, "ToLower", null),   // Convert property value to lowercase
                        "EndsWith", null, Expression.Constant(convertedValue.ToString().ToLower())  // Convert filter value to lowercase
                    );
                    break;

                case "equals":
                    filterExpression = Expression.Equal(propertyAccess, Expression.Constant(convertedValue));
                    break;
                // Add more cases for other match modes
                default:
                    filterExpression = Expression.Equal(propertyAccess, Expression.Constant(convertedValue));
                    break;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            return query.Where(lambda);
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (targetType == typeof(string))
            {
                return value.ToString();
            }
            else if (targetType == typeof(bool))
            {
                if (bool.TryParse(value.ToString(), out bool parsedValue))
                {
                    return parsedValue;
                }
                return false; // Default to false if parsing fails
            }
            else if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value.ToString());
            }
            else
            {
                return Convert.ChangeType(value, targetType);
            }

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
                }

                entities = FindByCondition(filters, parameters.OrderBy);
            }
            else
            {
                entities = FindAll();
            }

            var pagedResult = await entities.ToPagedListAsync(parameters.PageNumber, parameters.PageSize).ConfigureAwait(false);
            var destination = mapper.Map<List<TDto>>(pagedResult);

            
            var test = new StaticPagedList<TDto>(destination, pagedResult.GetMetaData());
            
            var pagedResponse = test.ToPagedResponse(entities.Count());

            return pagedResponse;
        }

        public IQueryable<T> FindByCondition(List<DataTableFilterMetaData>? filters, string orderBy)
        {
            var query = RepositoryContext.Set<T>();

            // Apply filters and sorting
            var filteredAndSortedQuery = ApplyFiltersAndSort(query, filters, orderBy);

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
}