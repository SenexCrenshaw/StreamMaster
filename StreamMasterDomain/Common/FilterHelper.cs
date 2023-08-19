using StreamMasterDomain.Filtering;

using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace StreamMasterDomain.Common;

public static class FilterHelper<T> where T : class
{
    private static readonly ConcurrentDictionary<Type, ParameterExpression> ParameterCache = new();
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> PropertyCache = new();

    public static IQueryable<T> ApplyFiltersAndSort(IQueryable<T> query, List<DataTableFilterMetaData>? filters, string orderBy)
    {
        if (filters != null)
        {
            // Apply filters
            foreach (DataTableFilterMetaData filter in filters)
            {
                query = FilterHelper<T>.ApplyFilter(query, filter);
            }
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = query.OrderBy(orderBy);
        }

        return query;
    }
    public static IQueryable<T> ApplyFilter(IQueryable<T> query, DataTableFilterMetaData filter)
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

    private static Expression CreateStringMatchExpression(DataTableFilterMetaData filter, Expression propertyAccess, string matchMode)
    {
        return Expression.Call(
                        propertyAccess,
                        matchMode, null, Expression.Constant(ConvertValue(filter.Value, typeof(string)))  // Convert filter value to lowercase
                    );
    }

    private static Expression CreateChannelGroupsExpression(DataTableFilterMetaData filter, Expression propertyAccess)
    {

        string[] channelGroups = JsonSerializer.Deserialize<string[]>(filter.Value.ToString());
        List<Expression> containsExpressions = new();
        foreach (string group in channelGroups)
        {
            MethodCallExpression containsCall = Expression.Call(propertyAccess, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(group.Trim()));
            containsExpressions.Add(containsCall);
        }

        Expression filterExpression = containsExpressions[0];
        for (int i = 1; i < containsExpressions.Count; i++)
        {
            filterExpression = Expression.OrElse(filterExpression, containsExpressions[i]);
        }

        return filterExpression;
    }

    private static object ConvertValue(object value, Type targetType)
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
}
