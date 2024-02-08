using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Filtering;

using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace StreamMaster.Domain.Common;

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
        string t = query.ToQueryString();
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
            PropertyCache[propertyKey] = property ?? throw new ArgumentException($"Property {filter.FieldName} not found on type {typeof(T).FullName}");
        }

        Expression propertyAccess = Expression.Property(parameter, property);

        Expression filterExpression = CreateArrayExpression(filter, propertyAccess);
        //filter.MatchMode switch
        //{
        //    //case "channelGroups":
        //    //    filterExpression = CreateArrayExpression(filter, propertyAccess, filter.MatchMode);
        //    //    break;
        //    //case "contains":
        //    //case "startsWith":
        //    //case "endsWith":
        //    //    filterExpression = CreateArrayExpression(filter, propertyAccess, filter.MatchMode);
        //    //    break;
        //    "equals" => Expression.Equal(propertyAccess, Expression.Constant(ConvertValue(filter.Value, property.PropertyType))),
        //    _ => CreateArrayExpression(filter, propertyAccess),
        //};
        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
        return query.Where(lambda);
    }

    private static MethodInfo? GetMethodCaseInsensitive(Type type, string methodName, Type[] parameterTypes)
    {
        return type.GetMethods()
                   .FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase)
                                        && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
    }

    public static MethodCallExpression StringExpression(string MatchMode, Expression stringExpression, Expression searchStringExpression)
    {
        MethodInfo? methodInfoString = GetMethodCaseInsensitive(typeof(string), MatchMode, [typeof(string)]);

        return methodInfoString == null
            ? throw new InvalidOperationException("No suitable Contains method found.")
            : Expression.Call(stringExpression, methodInfoString, searchStringExpression);
    }

    private static Expression CreateArrayExpression(DataTableFilterMetaData filter, Expression propertyAccess)
    {
        string stringValue = filter.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(filter.MatchMode))
        {
            filter.MatchMode = "contains";
        }
        else if (filter.MatchMode == "channelGroupsMatch")
        {
            filter.MatchMode = "equals";
        }

        List<Expression> containsExpressions = [];

        if (stringValue.StartsWith("[\"") && stringValue.EndsWith("\"]"))
        {
            string[] values = JsonSerializer.Deserialize<string[]>(stringValue) ?? Array.Empty<string>();
            foreach (string value in values)
            {
                if (propertyAccess.Type == typeof(int))
                {
                    string newValue = filter.Value.ToString()[2..^2];
                    BinaryExpression equalExpression = Expression.Equal(propertyAccess, Expression.Constant(int.Parse(newValue)));
                    containsExpressions.Add(equalExpression);
                }
                else
                {
                    MethodCallExpression containsCall = StringExpression(filter.MatchMode, propertyAccess, Expression.Constant(value));
                    containsExpressions.Add(containsCall);
                }
            }
        }
        else
        {
            if (propertyAccess.Type == typeof(int))
            {
                string newValue = filter.Value.ToString()[2..^2];
                BinaryExpression equalExpression = Expression.Equal(propertyAccess, Expression.Constant(int.Parse(newValue)));
                containsExpressions.Add(equalExpression);
            }
            if (propertyAccess.Type == typeof(bool))
            {
                if (filter.Value != null)
                {
                    bool newValue = bool.TryParse(filter.Value.ToString(), out bool parsedValue) && parsedValue;
                    BinaryExpression equalExpression = Expression.Equal(propertyAccess, Expression.Constant(newValue));
                    containsExpressions.Add(equalExpression);
                }
            }
            else
            {
                MethodCallExpression containsCall = StringExpression(filter.MatchMode, propertyAccess, Expression.Constant(stringValue));
                containsExpressions.Add(containsCall);
            }
        }

        Expression filterExpression = containsExpressions[0];
        for (int i = 1; i < containsExpressions.Count; i++)
        {
            filterExpression = Expression.OrElse(filterExpression, containsExpressions[i]);
        }

        return filterExpression;
    }

    private static object? ConvertValue(object value, Type? targetType)
    {
        // Handle null values immediately
        if (value == null || targetType == null)
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