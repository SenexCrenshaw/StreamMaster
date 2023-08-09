using StreamMasterDomain.Filtering;

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Diagnostics.Metrics;

namespace StreamMasterInfrastructureEF.Helpers;

public static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>> Like<T>(Expression<Func<T, string>> prop, string keyword)
    {
        var concatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });
        return Expression.Lambda<Func<T, bool>>(
            Expression.Call(
                typeof(DbFunctionsExtensions),
                nameof(DbFunctionsExtensions.Like),
                null,
                Expression.Constant(EF.Functions),
                prop.Body,
                Expression.Add(
                    Expression.Add(
                        Expression.Constant("%"),
                        Expression.Constant(keyword),
                        concatMethod),
                    Expression.Constant("%"),
                    concatMethod)),
            prop.Parameters);
    }

    public static Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity>(
        List<DataTableFilterMetaData> filters) where TEntity : class
    {
        Expression<Func<TEntity, bool>>? baseExpression = null;

        foreach (var filter in filters.Where(a => !string.IsNullOrEmpty(a.FieldName)))
        {
            PropertyInfo propertyInfo = typeof(TEntity).GetProperties()
    .FirstOrDefault(p => string.Equals(p.Name, filter.FieldName, StringComparison.OrdinalIgnoreCase));

            if (propertyInfo == null)
            {
                // Handle case when property is not found
                continue;
            }

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression propertyAccess = Expression.Property(parameter, propertyInfo);

            Expression? value = null;

            if (filter.ValueType == "string")
            {
                value = Expression.Constant(filter.Value.ToString());
            }
            else if (filter.ValueType == "boolean")
            {
                var test = filter.Value.ToString();
                if (test == null || test.ToLower() == "false")
                {
                    value = Expression.Constant(false);
                }
                else
                {
                    value = Expression.Constant(true);
                }
            }
            else if (filter.ValueType == "number")
            {
                value = Expression.Constant((int)filter.Value);
            }

            if (value == null)
            {
                continue;
            }
            Expression filterExpression = null;

            switch (filter.MatchMode)
            {
                case "contains":
                    filterExpression = Expression.Call(propertyAccess, "Contains", null, value);
                    break;

                case "startsWith":
                    filterExpression = Expression.Call(propertyAccess, "StartsWith", null, value);
                    break;

                case "endsWith":
                    filterExpression = Expression.Call(propertyAccess, "EndsWith", null, value);
                    break;

                case "equals":
                    filterExpression = Expression.Equal(propertyAccess, value);
                    break;
                // Add more cases for other match modes
                default:
                    break;
            }

            if (filterExpression != null)
            {
                if (baseExpression != null)
                {
                    baseExpression = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(baseExpression.Body, filterExpression), parameter);
                }
                else
                {
                    baseExpression = Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
                }
            }
            
        }

        return baseExpression == null ? _ => true : baseExpression;
    }
}