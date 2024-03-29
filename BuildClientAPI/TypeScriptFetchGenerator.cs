using System.Text;

public static class TypeScriptFetchGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        if (namespaceName == "SMChannels")

        {
            int aaa = 1;
        }
        methods = methods.Where(a => a.Name.StartsWith("Get")).ToList();
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

            string a = method.IsGetPaged ? CreateGetPaged(method) : CreateGet(method);

            content.AppendLine(a);
        }
        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string CreateGet(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine($"export const fetch{method.Name} = createAsyncThunk('cache/get{method.Name}', async (_: void, thunkAPI) => {{");
        content.AppendLine("  try {");
        content.AppendLine($"    const response = await {method.Name}();");
        content.AppendLine("    return { value: response };");
        content.AppendLine("  } catch (error) {");
        content.AppendLine("    console.error('Failed to fetch', error);");
        content.AppendLine("    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });");
        content.AppendLine("  }");
        content.AppendLine("});");
        return content.ToString();
    }

    private static string CreateGetPaged(MethodDetails method)
    {
        StringBuilder content = new();

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
        return content.ToString();
    }
}
