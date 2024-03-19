using System.Text;

public static class TypeScriptFetchGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        methods = methods.Where(a => a.Name.StartsWith("GetPaged")).ToList();
        if (methods.Count == 0)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return;
        }

        // Loop through each method to generate fetch functions
        foreach (MethodDetails method in methods)
        {
            // Add necessary imports
            string importLine = $"import {{ {method.Name} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Commands';";
            if (!content.ToString().Contains(importLine))
            {
                content.AppendLine(importLine);
            }
        }

        content.AppendLine("import { createAsyncThunk } from '@reduxjs/toolkit';");
        content.AppendLine();

        foreach (MethodDetails method in methods)
        {
            // Assume method.Parameters has the required format "type name"
            string parameterType = method.Parameters.Split(": ").FirstOrDefault()?.Trim();
            string actionName = method.Name[..1].ToLower() + method.Name[1..]; // CamelCase the action name

            content.AppendLine($"export const fetch{method.Name} = createAsyncThunk('cache/get{method.Name}', async (query: string, thunkAPI) => {{");
            content.AppendLine("  try {");
            content.AppendLine("    const params = JSON.parse(query);");
            content.AppendLine($"    const response = await {method.Name}(params);");
            content.AppendLine("    return { query: query, value: response };");
            content.AppendLine("  } catch (error) {");
            content.AppendLine("    console.error('Failed to fetch', error);");
            content.AppendLine("    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });");
            content.AppendLine("  }");
            content.AppendLine("});");
            content.AppendLine();
        }

        File.WriteAllText(filePath, content.ToString());
    }
}
