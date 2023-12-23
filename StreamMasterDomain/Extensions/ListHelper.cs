using System.Reflection;
using System.Text.RegularExpressions;

namespace StreamMasterDomain.Extensions;

public static class ListHelper
{
    public static List<T> GetMatchingProperty<T>(List<T> list, string propertyName, string regex)
    {
        List<T> matchedObjects = [];
        Regex rgx = new(regex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

        PropertyInfo? property = typeof(T).GetProperty(propertyName);
        if (property == null)
        {
            throw new ArgumentException("No such property found", "propertyName");
        }

        foreach (T? obj in list)
        {
            object? value = property.GetValue(obj, null);
            if (value != null)
            {
                string stringValue = value.ToString();
                if (rgx.IsMatch(stringValue))
                {
                    matchedObjects.Add(obj);
                }
            }
        }

        return matchedObjects;
    }
}
