using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
namespace StreamMaster.Domain.Filtering;
public static partial class FilterHelper<T> where T : class
{
    private static readonly ConcurrentDictionary<Type, Lazy<ParameterExpression>> ParameterCache = new();
    private static readonly ConcurrentDictionary<(Type, string), Lazy<PropertyInfo>> PropertyCache = new();

    public static readonly List<string> FiltersToIgnore = ["inSG", "notInSG"];

    public static IQueryable<T> ApplyFiltersAndSort(IQueryable<T> query, List<DataTableFilterMetaData>? filters, string orderBy, bool forceToLower = false)
    {
        if (filters?.Any(a => !FiltersToIgnore.Contains(a.MatchMode)) == true)
        {
            query = ApplyFilter(query, filters, forceToLower);
        }

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = query.OrderBy(orderBy);
        }

        // Log the query for debugging purposes
        // Replace this with your logging system
        Console.WriteLine(query.ToQueryString());

        return query;
    }

    public static IQueryable<T> ApplyFilter(IQueryable<T> query, List<DataTableFilterMetaData> filters, bool forceToLower)
    {
        if (filters == null || filters.Count == 0)
        {
            return query;
        }

        ParameterExpression parameter = GetOrAddParameterExpression(typeof(T));
        Dictionary<string, List<Expression>> filterExpressions = [];

        foreach (DataTableFilterMetaData filter in filters)
        {
            if (FiltersToIgnore.Contains(filter.MatchMode))
            {
                continue;
            }

            PropertyInfo property = GetOrAddPropertyInfo(typeof(T), filter.FieldName);

            MemberExpression propertyAccess = Expression.Property(parameter, property);
            Expression filterExpression = CreateArrayExpression(filter, propertyAccess, forceToLower);

            if (!filterExpressions.TryGetValue(property.Name, out List<Expression>? expressions))
            {
                expressions = [];
                filterExpressions.Add(property.Name, expressions);
            }

            expressions.Add(filterExpression);
        }

        List<Expression> combinedPropertyExpressions = [.. filterExpressions.Values.Select(expressions => expressions.Aggregate(Expression.OrElse))];

        Expression finalExpression = combinedPropertyExpressions.Aggregate(Expression.AndAlso);
        Expression<Func<T, bool>> finalLambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);

        return query.Where(finalLambda);
    }

    private static ParameterExpression GetOrAddParameterExpression(Type type)
    {
        return ParameterCache.GetOrAdd(type, t => new Lazy<ParameterExpression>(() => Expression.Parameter(t, "entity"))).Value;
    }

    private static PropertyInfo GetOrAddPropertyInfo(Type type, string fieldName)
    {
        return PropertyCache.GetOrAdd((type, fieldName), key =>
        {
            (Type keyType, string keyFieldName) = key; // Deconstruct the tuple

            PropertyInfo? property = keyType.GetProperties()
                .FirstOrDefault(p => p.Name.EqualsIgnoreCase(keyFieldName));

            return new Lazy<PropertyInfo>(() => property ?? throw new ArgumentException(
                $"Property '{keyFieldName}' not found on type '{keyType.FullName}'."));
        }).Value;
    }

    private static Expression CreateArrayExpression(DataTableFilterMetaData filter, Expression propertyAccess, bool forceToLower)
    {
        string stringValue = filter.Value?.ToString() ?? string.Empty;
        filter.MatchMode ??= FilterModes.Contains;

        List<Expression> containsExpressions = [];
        string[] values = stringValue.StartsWith('[') ? ConvertToArray(stringValue) : [stringValue];

        foreach (string? value in values)
        {
            Expression expression = propertyAccess.Type switch
            {
                Type type when type == typeof(int) => Expression.Equal(propertyAccess, Expression.Constant(int.Parse(value))),
                Type type when type == typeof(bool) => Expression.Equal(propertyAccess, Expression.Constant(bool.Parse(value))),
                _ => CreateStringExpression(filter.MatchMode, propertyAccess, value, forceToLower)
            };

            containsExpressions.Add(expression);
        }

        return containsExpressions.Aggregate(Expression.OrElse);
    }

    private static MethodCallExpression CreateStringExpression(string matchMode, Expression propertyAccess, string value, bool forceToLower)
    {
        propertyAccess = ApplyCaseSensitivity(propertyAccess, forceToLower);
        MethodInfo? methodInfo = GetMethodCaseInsensitive(typeof(string), matchMode, [typeof(string)]);
        return methodInfo == null
            ? throw new InvalidOperationException($"Method '{matchMode}' not found on type 'string'.")
            : Expression.Call(propertyAccess, methodInfo, Expression.Constant(value));
    }

    private static Expression ApplyCaseSensitivity(Expression propertyAccess, bool forceToLower)
    {
        if (!forceToLower)
        {
            return propertyAccess;
        }

        MethodInfo? toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        return toLowerMethod == null
            ? throw new InvalidOperationException("Method 'ToLower' not found on type 'string'.")
            : (Expression)Expression.Call(propertyAccess, toLowerMethod);
    }

    private static MethodInfo? GetMethodCaseInsensitive(Type type, string methodName, Type[] parameterTypes)
    {
        if (methodName == FilterModes.NotContains)
        {
            methodName = FilterModes.Contains;
        }

        return type.GetMethods()
            .FirstOrDefault(m => m.Name.EqualsIgnoreCase(methodName)
                                 && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
    }

    private static string[] ConvertToArray(string stringValue)
    {
        if (ArrayRegex.IsMatch(stringValue))
        {
            stringValue = ArrayValueRegex.Replace(stringValue, "\"$1\"");
        }

        return JsonSerializer.Deserialize<string[]>(stringValue.Replace("'", "\""))
               ?? throw new InvalidOperationException("Invalid JSON format.");
    }

    private static readonly Regex ArrayRegex = MyRegex();
    private static readonly Regex ArrayValueRegex = MyRegex1();

    [GeneratedRegex(@"\[\s*(-?\d+)\s*(,\s*-?\d+\s*)*\]")]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"(-?\d+)")]
    private static partial Regex MyRegex1();

    private static class FilterModes
    {
        public const string Contains = "contains";
        public const string NotContains = "notContains";
        public new const string Equals = "equals";
        public const string ChannelGroupsMatch = "channelGroupsMatch";
    }
}
