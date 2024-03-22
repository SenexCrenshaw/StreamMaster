using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Provides utilities for converting C# parameter and type information into TypeScript equivalents.
/// </summary>
public static class ParameterConverter2
{
    /// <summary>
    /// Converts a C# parameters string to a TypeScript parameters string.
    /// </summary>
    /// <param name="csharpParameters">The C# parameters string to convert.</param>
    /// <returns>A TypeScript parameters string formatted as "name: type, ...".</returns>
    public static string ConvertCSharpParametersToTypeScript(string csharpParameters)
    {
        if (string.IsNullOrWhiteSpace(csharpParameters))
        {
            return string.Empty;
        }

        IEnumerable<string> types = csharpParameters.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where((_, index) => index % 2 == 0);
        IEnumerable<string> names = csharpParameters.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where((_, index) => index % 2 != 0);

        string tsParameters = types.Zip(names, (type, name) => $"{name}: {MapCSharpTypeToTypeScript(type)}")
            .Aggregate((current, next) => $"{current}, {next}");

        return tsParameters;
    }

    public static string ParamsToCSharp(MethodInfo method)
    {
        // string.Join(", ", method.GetParameters().Select(p => $"{ParameterConverter.CleanupTypeName(ParameterConverter.GetTypeFullNameForParameter(p.ParameterType))} {p.Name}"));
        StringBuilder stringBuilder = new();
        ParameterInfo[] ps = method.GetParameters();
        foreach (ParameterInfo p in ps)
        {
            string? name = p.Name;
            Type pType = p.ParameterType;
            string csSharptsTypeFullName = GetTypeFullNameForParameter(p.ParameterType);
            string csSharptsType = CleanupTypeName(csSharptsTypeFullName);

            stringBuilder.Append($"{csSharptsType} {name}");
        }
        string ret = string.Join(", ", stringBuilder);

        return ret;
    }

    public static string CSharpParamToTS(MethodInfo method)
    {
        if (method.Name == "DeleteAllSMChannelsFromParameters")
        {
            int aa = 1;
        }
        StringBuilder stringBuilder = new();
        ParameterInfo[] ps = method.GetParameters();
        foreach (ParameterInfo p in ps)
        {
            string? name = p.Name;
            Type pType = p.ParameterType;
            string tsTypeFullName = GetTypeFullNameForParameter(pType);
            string tt = MapCSharpTypeToTypeScript(tsTypeFullName);
            string tsType = GetLastPartOfTypeName(tt);
            tsType = FixUpTSType(tsType);
            stringBuilder.Append($"{name}: {tsType}");

        }
        string ret = string.Join(", ", stringBuilder);

        return ret;
    }

    public static string FixUpTSType(string tsType)
    {
        string pattern = @"\b\w*Parameters(?!\:)\b";
        tsType = Regex.Replace(tsType, pattern, "QueryStringParameters");
        return tsType;
    }

    public static string GetLastPartOfTypeName(string fullTypeName)
    {
        int lastIndex = fullTypeName.LastIndexOf('.');
        return lastIndex >= 0 ? fullTypeName[(lastIndex + 1)..] : fullTypeName;
    }
    public static string? IsTSGeneric(string csharpType)
    {
        if (csharpType.Contains(":"))
        {
            csharpType = csharpType[(csharpType.IndexOf(": ") + 2)..];
        }
        string toTest = csharpType.ToLower();

        if (toTest == "system.string" || toTest.StartsWith("string"))
        {
            return null;
        }
        else if (toTest == "system.int32" || toTest.StartsWith("number") || toTest == "int32")
        {
            return null;
        }
        else if (toTest is "system.double" or "double")
        {
            return null;
        }
        else if (toTest is "system.boolean" or "boolean")
        {
            return null;
        }

        if (csharpType.EndsWith("[]"))
        {
            csharpType = csharpType[..^2];
        }

        return csharpType;
    }

    public static string MapCSharpTypeToTypeScript(string csharpType)
    {
        // Handle basic types directly
        switch (csharpType)
        {
            case "System.String": return "string";
            case "System.Int32": return "number";
            case "System.Double": return "number";
            case "System.Boolean": return "boolean";
                // Add more basic type mappings as needed
        }

        // Handle generics and collections
        if (csharpType.StartsWith("System.Collections.Generic.List<") || csharpType.StartsWith("System.Collections.Generic.IEnumerable<"))
        {
            string innerType = ExtractInnermostType(csharpType);
            string tsInnerType = MapCSharpTypeToTypeScript(innerType); // Recursively convert the inner type
            return $"{tsInnerType}[]";
        }

        // Default fallback for unmapped types
        return csharpType;
    }

    public static string CleanupTypeName(string fullTypeName)
    {
        // Pattern matches generic types like "System.Collections.Generic.List<System.Int32>"
        string genericTypePattern = @"System\.Collections\.Generic\.(?<type>[^\[\<]+)\<(?<innerType>.+)\>";

        // Use Regex to simplify generic type names
        Match match = Regex.Match(fullTypeName, genericTypePattern);
        if (match.Success)
        {
            string type = match.Groups["type"].Value; // e.g., "List"
            string innerType = match.Groups["innerType"].Value; // e.g., "System.Int32"

            // Recursive cleanup in case of nested generics
            string cleanedInnerType = CleanupTypeName(innerType);

            return $"{type}<{cleanedInnerType}>";
        }

        // Simplify common system types
        string simplifiedType = fullTypeName
            .Replace("System.Int32", "int")
            .Replace("System.String", "string")
            .Replace("System.Double", "Double")
            .Replace("System.Boolean", "Boolean")
            // Add more replacements as needed
            ;

        return GetLastPartOfTypeName(simplifiedType);
    }

    /// <summary>
    /// Extracts the innermost type from a generic type string.
    /// </summary>
    /// <param name="genericTypeString">The generic type string to parse.</param>
    /// <returns>The innermost type if found; otherwise, returns the original string.</returns>
    public static string ExtractInnermostType(string genericTypeString)
    {
        int start = genericTypeString.IndexOf('<') + 1;
        int end = genericTypeString.LastIndexOf('>');
        if (start > 0 && end > start)
        {
            return genericTypeString[start..end];
            //return innerType switch
            //{
            //    "System.String" => "string",
            //    "System.Int32" => "number",
            //    "System.Double" => "number",
            //    "System.Boolean" => "boolean",
            //    // Handle other cases as necessary
            //    _ => innerType,// Fallback or further processing for other types
            //};
        }
        return genericTypeString; // Fallback for non-generic types
    }


    public static string GetTypeFullNameForParameter(Type type)
    {
        if (type.IsGenericType)
        {
            string typeName = type.GetGenericTypeDefinition().FullName;
            typeName = typeName[..typeName.IndexOf('`')]; // Remove the backtick and generic parameter count
            string genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetTypeFullNameForParameter)); // Recursive call for generic arguments
            //genericArguments = MapCSharpTypeToTypeScript(genericArguments);
            //if (typeName.StartsWith("System.Collections.Generic."))
            //{
            //    typeName = typeName.Replace("System.Collections.Generic.", "");


            //}
            //string test = MapCSharpTypeToTypeScript($"{typeName}<{genericArguments}>");
            //return test;
            return $"{typeName}<{genericArguments}>";
        }
        else
        {
            string ret = type.FullName ?? type.Name;
            if (ret.Contains("+"))
            {
                ret = type.Name;
            }

            return ret;
        }
    }

    /// <summary>
    /// Handles mapping of generic types to TypeScript.
    /// </summary>
    /// <param name="genericType">The generic type string.</param>
    /// <returns>A TypeScript representation of the generic type.</returns>
    /// <remarks>This method serves as a placeholder for future implementation of complex generic type handling.</remarks>
    private static string HandleGenericTypes(string genericType)
    {
        // Placeholder for additional generic type handling logic
        return genericType;
    }
}
