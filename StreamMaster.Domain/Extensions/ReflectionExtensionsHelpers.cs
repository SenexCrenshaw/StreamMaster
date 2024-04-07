using System.Linq.Expressions;

public static class ReflectionExtensionsHelpers
{
    /// <summary>
    /// Updates a property of the target object if the value differs from the corresponding property in the source object.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TTarget">The type of the target object.</typeparam>
    /// <param name="target">The target object whose property may be updated.</param>
    /// <param name="propertyLambda">A lambda expression representing the property to update.</param>
    /// <param name="source">The source object from which to retrieve the new value.</param>
    /// <param name="ret">A collection where changes are tracked; modified if the property value is updated.</param>
    /// <returns>True if the property value was updated; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the property name could not be extracted from the lambda expression or if the property does not exist on one of the objects.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the target object does not have an 'Id' property.</exception>
    /// <remarks>
    /// This method uses reflection to compare and potentially update the value of a specified property on the target object,
    /// based on the value of the corresponding property on the source object. It is useful for synchronizing values across
    /// objects of different types that share property names. The method assumes that both the source and target objects
    /// have a property with the name specified by the <paramref name="propertyLambda"/> parameter.
    /// </remarks>
    public static bool UpdatePropertyIfDifferent<TSource, TTarget>(
        this TTarget target,
        Expression<Func<TTarget, object>> propertyLambda,
        TSource source,
        ICollection<FieldData> ret)
        where TTarget : class
        where TSource : class
    {
        // Extracting property name from the lambda expression
        MemberExpression? member = propertyLambda.Body as MemberExpression ??
                     (propertyLambda.Body as UnaryExpression)?.Operand as MemberExpression;
        if (member == null)
        {
            throw new ArgumentException("Property name could not be extracted.", nameof(propertyLambda));
        }
        string propertyName = member.Member.Name;

        // Using reflection to get the property from both source and target
        System.Reflection.PropertyInfo? targetPropertyInfo = typeof(TTarget).GetProperty(propertyName);
        System.Reflection.PropertyInfo? sourcePropertyInfo = typeof(TSource).GetProperty(propertyName);
        if (targetPropertyInfo == null || sourcePropertyInfo == null)
        {
            throw new ArgumentException($"Property {propertyName} not found on one of the objects.", propertyName);
        }

        // Getting the current values from both target and source
        object? targetValue = targetPropertyInfo.GetValue(target);
        object? sourceValue = sourcePropertyInfo.GetValue(source);

        if (!EqualityComparer<object>.Default.Equals(targetValue, sourceValue))
        {
            targetPropertyInfo.SetValue(target, sourceValue);

            // Dynamically accessing the Id property of the target
            System.Reflection.PropertyInfo? idProperty = typeof(TTarget).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("The target object does not have an Id property.");
            }
            string idValue = idProperty.GetValue(target)?.ToString() ?? "Unknown";

            ret.Add(new FieldData(typeof(TTarget).Name, idValue, propertyName, sourceValue));
            return true;
        }
        return false;
    }
}