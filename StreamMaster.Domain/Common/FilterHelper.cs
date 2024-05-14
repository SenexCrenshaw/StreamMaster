using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Filtering;

using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Common;

public static class FilterHelper<T> where T : class
{
    private static readonly ConcurrentDictionary<Type, ParameterExpression> ParameterCache = new();
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo> PropertyCache = new();

    public static readonly List<string> FiltersToIgnore = ["inSG", "notInSG"];

    public static IQueryable<T> ApplyFiltersAndSort(IQueryable<T> query, List<DataTableFilterMetaData>? filters, string orderBy, bool forceToLower = false)
    {
        if (filters != null)
        {
            if (filters.Any(a => !FiltersToIgnore.Contains(a.MatchMode)))
            {
                query = FilterHelper<T>.ApplyFilter(query, filters, forceToLower);
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

    public static IQueryable<T> ApplyFilter(IQueryable<T> query, List<DataTableFilterMetaData> filters, bool forceToLower)
    {
        if (filters == null || filters.Count == 0)
        {
            return query;
        }

        Dictionary<string, List<Expression>> filterExpressions = new();
        if (!ParameterCache.TryGetValue(typeof(T), out ParameterExpression? parameter))
        {
            parameter = Expression.Parameter(typeof(T), "entity");
            ParameterCache[typeof(T)] = parameter;
        }

        foreach (DataTableFilterMetaData filter in filters)
        {
            if (filter.MatchMode.Equals("inSG"))
            {
                continue;
            }
            (Type, string FieldName) propertyKey = (typeof(T), filter.FieldName);
            if (!PropertyCache.TryGetValue(propertyKey, out PropertyInfo? property))
            {
                property = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, filter.FieldName, StringComparison.OrdinalIgnoreCase));
                PropertyCache[propertyKey] = property ?? throw new ArgumentException($"Property {filter.FieldName} not found on type {typeof(T).FullName}");
            }

            Expression propertyAccess = Expression.Property(parameter, property);
            Expression filterExpression = CreateArrayExpression(filter, propertyAccess, forceToLower);
            if (!filterExpressions.TryGetValue(property.Name, out List<Expression>? expressions))
            {
                expressions = new List<Expression>();
                filterExpressions.Add(property.Name, expressions);
            }
            expressions.Add(filterExpression);
        }

        List<Expression> combinedPropertyExpressions = new List<Expression>();
        foreach (var propExpressions in filterExpressions.Values)
        {
            Expression combinedExpression = propExpressions[0];
            for (int i = 1; i < propExpressions.Count; i++)
            {
                combinedExpression = Expression.OrElse(combinedExpression, propExpressions[i]);
            }
            combinedPropertyExpressions.Add(combinedExpression);
        }

        Expression finalExpression = combinedPropertyExpressions[0];
        for (int i = 1; i < combinedPropertyExpressions.Count; i++)
        {
            finalExpression = Expression.AndAlso(finalExpression, combinedPropertyExpressions[i]);
        }

        Expression<Func<T, bool>> finalLambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);


        return query.Where(finalLambda);
    }

    private static MethodInfo? GetMethodCaseInsensitive(Type type, string methodName, Type[] parameterTypes)
    {
        if (methodName == "notContains")
        {
            methodName = "contains";

        }
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


    private static List<string> ConvertToJsonStringList(string jsonString)
    {
        try
        {
            var stringList = JsonSerializer.Deserialize<List<string>>(jsonString);
            if (stringList == null) throw new JsonException("Deserialization returned null.");
            return stringList;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to convert to List<string>: {ex.Message}");
            return null;
        }
    }

    private static List<int> ConvertToJsonIntList(string jsonString)
    {
        try
        {
            var intList = JsonSerializer.Deserialize<List<int>>(jsonString);
            if (intList == null) throw new JsonException("Deserialization returned null.");
            return intList;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to convert to List<int>: {ex.Message}");
            return null;
        }
    }

    private static Expression CreateArrayExpression(DataTableFilterMetaData filter, Expression propertyAccess, bool forceToLower)
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

        if (stringValue.StartsWith("[\"") && stringValue.EndsWith("\"]")
           ||
        stringValue.StartsWith("[") && stringValue.EndsWith("")
            )
        {
            string[] values = ConvertToArray(stringValue);
            foreach (string value in values)
            {
                if (propertyAccess.Type == typeof(int))
                {
                    BinaryExpression equalExpression = Expression.Equal(propertyAccess, Expression.Constant(int.Parse(value)));
                    containsExpressions.Add(equalExpression);
                }
                else
                {
                    if (forceToLower)
                    {
                        MethodInfo? methodInfoString = GetMethodCaseInsensitive(typeof(string), filter.MatchMode, new[] { typeof(string) });
                        MethodCallExpression toLowerCall = Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        MethodCallExpression matchCall = Expression.Call(toLowerCall, methodInfoString, Expression.Constant(value.ToLower()));
                        containsExpressions.Add(matchCall);
                    }
                    else
                    {

                        MethodCallExpression containsCall = StringExpression(filter.MatchMode, propertyAccess, Expression.Constant(value));
                        if (filter.MatchMode.Equals("notContains"))
                        {
                            UnaryExpression notContainsCall = Expression.Not(containsCall);
                            containsExpressions.Add(containsCall);

                        }
                        else
                        {
                            containsExpressions.Add(containsCall);
                        }

                    }
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
                if (forceToLower)
                {
                    MethodInfo? methodInfoString = GetMethodCaseInsensitive(typeof(string), filter.MatchMode, new[] { typeof(string) });
                    MethodCallExpression toLowerCall = Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    MethodCallExpression matchCall = Expression.Call(toLowerCall, methodInfoString, Expression.Constant(stringValue.ToLower()));
                    containsExpressions.Add(matchCall);
                }
                else
                {
                    MethodCallExpression containsCall = StringExpression(filter.MatchMode, propertyAccess, Expression.Constant(stringValue));
                    if (filter.MatchMode.Equals("notContains"))
                    {
                        UnaryExpression notContainsCall = Expression.Not(containsCall);
                        containsExpressions.Add(notContainsCall);

                    }
                    else
                    {
                        containsExpressions.Add(containsCall);
                    }
                }
            }
        }

        Expression filterExpression = containsExpressions[0];
        for (int i = 1; i < containsExpressions.Count; i++)
        {
            filterExpression = Expression.OrElse(filterExpression, containsExpressions[i]);
        }

        return filterExpression;
    }

    private static string[] ConvertToArray(string stringValue)
    {
        // Check if the string contains unquoted numbers
        if (Regex.IsMatch(stringValue, @"\[\s*(\d+)\s*(,\s*\d+\s*)*\]"))
        {
            // Add quotes around numbers
            stringValue = Regex.Replace(stringValue, @"(\d+)", "\"$1\"");
        }

        // Ensure all single quotes are replaced by double quotes
        string normalizedInput = stringValue.Replace("'", "\"");


        try
        {
            // Deserialize the string
            string[] values = JsonSerializer.Deserialize<string[]>(normalizedInput) ?? Array.Empty<string>();
            return values;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid JSON format.", ex);
        }
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