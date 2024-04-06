using System.Text;

public static class TypeScriptFetchGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string path)
    {

        if (namespaceName == "ChannelGroups")

        {
            int aaa = 1;
        }


        // Loop through each method to generate fetch functions
        foreach (MethodDetails method in methods.Where(a => a.IsGet))
        {
            StringBuilder content = new();
            content.AppendLine(GetImports(namespaceName, method));

            content.AppendLine(GetFetch(method));

            string fileName = $"{method.Name}Fetch.ts";
            string filePath = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(filePath, content.ToString());

        }

    }

    public static string GetImports(string namespaceName, MethodDetails method)
    {

        StringBuilder content = new();
        content.AppendLine($"import {{ {method.Name} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Commands';");
        if (!string.IsNullOrEmpty(method.TsParameter))
        {
            content.AppendLine($"import {{ {method.Name}Request }} from '../smapiTypes';");
        }

        content.AppendLine("import { createAsyncThunk } from '@reduxjs/toolkit';");
        content.AppendLine();

        return content.ToString();
    }

    public static string GetFetch(MethodDetails method)
    {

        StringBuilder content = new();

        string a = method.IsGetPaged ? CreateGetPaged(method) : CreateGet(method);

        content.AppendLine(a);

        return content.ToString();
    }

    private static string CreateGet(MethodDetails method)
    {
        StringBuilder content = new();
        if (method.Name.Contains("GetIcons"))
        {
            int aa = 1;

        }

        string param = string.IsNullOrEmpty(method.TsParameter) ? "void" : method.TsParameter;
        string paramName = string.IsNullOrEmpty(method.TsParameter) ? "_" : "param";
        string paramName2 = string.IsNullOrEmpty(method.TsParameter) ? "" : "param";

        content.AppendLine($"export const fetch{method.Name} = createAsyncThunk('cache/get{method.Name}', async ({paramName}: {param}, thunkAPI) => {{");
        content.AppendLine("  try {");
        content.AppendLine($"    console.log('Fetching {method.Name}');");
        content.AppendLine($"    const response = await {method.Name}({paramName2});");
        if (method.IsList)
        {
            content.AppendLine($"    console.log('Fetched {method.Name} ',response?.length);");
        }
        else
        {
            content.AppendLine($"    console.log('Fetched {method.Name}',response);");
        }


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
        content.AppendLine("    if (query === undefined) return;");
        content.AppendLine($"    console.log('Fetching {method.Name}');");
        content.AppendLine("    const params = JSON.parse(query);");
        content.AppendLine($"    const response = await {method.Name}(params);");
        content.AppendLine($"    console.log('Fetched {method.Name} ',response?.data.length);");
        content.AppendLine("    return { query: query, value: response };");
        content.AppendLine("  } catch (error) {");
        content.AppendLine("    console.error('Failed to fetch', error);");
        content.AppendLine("    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });");
        content.AppendLine("  }");
        content.AppendLine("});");
        return content.ToString();
    }
}
