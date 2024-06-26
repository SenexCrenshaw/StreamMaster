using Reinforced.Typings.Attributes;

using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace StreamMaster.Domain.Models;


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class FieldData
{

    public FieldData(string entity, object Parameter, object value)
    {
        string jsonString = JsonSerializer.Serialize(Parameter);
        Entity = entity;
        Id = jsonString;
        Value = value;
    }

    public FieldData(string entity, int id, string field, object? value)
    {
        Entity = entity;
        Id = id.ToString();
        Field = field;
        Value = value;
    }

    public FieldData(string entity, string id, string field, object? value)
    {
        Entity = entity;
        Id = id;
        Field = field;
        Value = value;
    }

    public FieldData(string entity, string id, Expression<Func<object>> propertyExpression)
    {
        (string propertyName, object value) = ExtractPropertyNameAndValue(propertyExpression);
        Entity = entity;
        Id = id;
        Field = propertyName;
        Value = value;
    }

    public FieldData(object entity, string propertyName)
    {
        Entity = ExtractAPIName(entity);
        Id = ExtractId(entity) ?? throw new InvalidOperationException("ID cannot be null.");
        Field = propertyName;
        Value = ExtractPropertyValue(entity, propertyName);
    }

    public FieldData(Expression<Func<object>> propertyExpression)
    {

        (string entity, string id, string propertyName, object value) = ExtractEntityPropertyNameAndValue(propertyExpression);
        Entity = entity;
        Id = id ?? throw new InvalidOperationException("ID cannot be null.");
        Field = propertyName;
        Value = value;
    }

    private static (string entity, string id, string propertyName, object value) ExtractEntityPropertyNameAndValue(Expression<Func<object>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression member)
        {
            object entity = ExtractEntityFromMemberExpression(member);
            return (ExtractAPIName(entity), ExtractId(entity), member.Member.Name, GetValue(propertyExpression));
        }
        else if (propertyExpression.Body is UnaryExpression unaryExp && unaryExp.Operand is MemberExpression memberExp)
        {
            object entity = ExtractEntityFromMemberExpression(memberExp);
            return (ExtractAPIName(entity), ExtractId(entity), memberExp.Member.Name, GetValue(propertyExpression));
        }

        throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }
    private static object ExtractEntityFromMemberExpression(MemberExpression memberExpression)
    {
        // Extract the constant expression that represents the target object ('entity')
        if (memberExpression.Expression is ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }
        else if (memberExpression.Expression is MemberExpression outerMemberExpression)
        {
            // Handle nested properties
            Delegate compiledLambda = Expression.Lambda(outerMemberExpression).Compile();
            return compiledLambda.DynamicInvoke();
        }

        throw new ArgumentException("Could not determine the target entity from the expression.");
    }

    private static string ExtractAPIName(object entity)
    {
        PropertyInfo? idProperty = entity.GetType().GetProperty("APIName");
        return idProperty != null ? (idProperty.GetValue(entity)?.ToString()) : entity.GetType().Name;
    }

    private static string ExtractId(object entity)
    {
        PropertyInfo? idProperty = entity.GetType().GetProperty("Id");
        return idProperty != null ? idProperty.GetValue(entity)?.ToString() ?? "NOTFOUND" : "NOTFOUND";
    }

    private static object? ExtractPropertyValue(object entity, string propertyName)
    {
        PropertyInfo? property = entity.GetType().GetProperty(propertyName);
        return property == null
            ? throw new ArgumentException($"Property '{propertyName}' not found on entity type '{entity.GetType().Name}'.")
            : property.GetValue(entity);
    }

    private static (string propertyName, object value) ExtractPropertyNameAndValue(Expression<Func<object>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression member)
        {
            return (member.Member.Name, GetValue(propertyExpression));
        }
        else if (propertyExpression.Body is UnaryExpression unaryExp && unaryExp.Operand is MemberExpression memberExp)
        {
            // Handle cases where the property is boxed into an object, causing a Convert expression type.
            return (memberExp.Member.Name, GetValue(propertyExpression));
        }

        throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }

    private static object GetValue(Expression<Func<object>> expression)
    {
        // Compiles the expression tree to a callable delegate and invokes it to get the value
        return expression.Compile().Invoke();
    }

    private static string GetPropertyName(Expression<Func<object>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression member)
        {
            return member.Member.Name;
        }
        else if (propertyExpression.Body is UnaryExpression unaryExp && unaryExp.Operand is MemberExpression memberExp)
        {
            // Handle cases where the property is boxed into an object, causing a Convert expression type.
            return memberExp.Member.Name;
        }

        throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }

    public string Entity { get; set; }
    public string Id { get; set; }
    public string Field { get; set; }
    public object Value { get; set; }
}

