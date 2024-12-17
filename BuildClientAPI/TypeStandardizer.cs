namespace BuildClientAPI
{
    public static class TypeStandardizer
    {
        private static readonly Dictionary<string, string> typeMappings = new()
        {
        { "String", "string" },
        { "Int32", "int" },
        { "Boolean", "bool" },
        // Add more mappings as needed
    };

        public static string GetStandardType(string originalType)
        {
            // Attempt to get the value from the dictionary; if found, return it, otherwise return the original type.
            if (typeMappings.TryGetValue(originalType, out string? standardType))
            {
                return standardType;
            }

            // Handling for generic task types, e.g., Task<Int32> to Task<int>
            if (originalType.StartsWith("System.Threading.Tasks.Task`1"))
            {
                string genericArgument = originalType.Split('[')[1].Split(']')[0]; // Extract the generic type
                string cleanGenericArgument = genericArgument.Replace("System.", ""); // Simplify the type name
                if (typeMappings.TryGetValue(cleanGenericArgument, out string? mappedType))
                {
                    return $"Task<{mappedType}>"; // Return the mapped Task type
                }
            }

            return originalType; // Return the original type if no mapping is found
        }
    }
}
