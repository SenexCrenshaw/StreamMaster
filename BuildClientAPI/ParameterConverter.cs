public static class ParameterConverter
{
    private const string AssemblyName = "StreamMaster.Application";
    public static string ExtractInnermostType(string genericTypeString)
    {
        int start = genericTypeString.LastIndexOf('<') + 1;
        int end = genericTypeString.LastIndexOf('>');

        if (start == 0 || end == -1 || end < start)
        {
            // No valid generic type found, return the original string or handle as error
            return genericTypeString;
        }

        return genericTypeString[start..end];
    }


    public static string ConvertCSharpParametersToTypeScript(string csharpParameters)
    {
        if (string.IsNullOrWhiteSpace(csharpParameters))
        {
            return string.Empty;
        }

        string tsParameters = csharpParameters
            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where((_, index) => index % 2 == 0) // Assumes even index is type, odd index is name
            .Zip( // Pair each type with its corresponding name
                csharpParameters
                    .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where((_, index) => index % 2 != 0),
                (type, name) => $"{name}: {MapCSharpTypeToTypeScript(type)}" // Map C# type to TypeScript type
            )
            .Aggregate((current, next) => $"{current}, {next}"); // Concatenate all parameters into a single string

        return tsParameters;
    }

    private static string MapCSharpTypeToTypeScript(string csharpType)
    {
        return csharpType switch
        {
            "string" => "string",
            "int" => "number",
            "double" => "number",
            "bool" => "boolean",
            _ => csharpType // Return the original C# type if no match is found
        };
    }
}
