using System.Text;

using BuildClientAPI.Models;
namespace BuildClientAPI.TS;
public static class TypeScriptFetchGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string path)
    {
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
        if (!string.IsNullOrEmpty(method.TsParameter) && !method.IsGetPaged)
        {
            content.AppendLine($"import {{ {method.Name}Request }} from '../smapiTypes';");
        }

        if (method.IsGet && (method.IsGetPaged || method.ParameterNames.Length != 0))
        {
            content.AppendLine("import { isSkipToken } from '@lib/common/isSkipToken';");
        }

        content.AppendLine("import { Logger } from '@lib/common/logger';");
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

        string param = string.IsNullOrEmpty(method.TsParameter) ? "void" : method.TsParameter;
        string paramName = string.IsNullOrEmpty(method.TsParameter) ? "_" : "param";
        string paramName2 = string.IsNullOrEmpty(method.TsParameter) ? "" : "param";

        content.AppendLine($"export const fetch{method.Name} = createAsyncThunk('cache/get{method.Name}', async ({paramName}: {param}, thunkAPI) => {{");
        content.AppendLine("  try {");
        if (paramName != "_")
        {
            content.AppendLine($"    if (isSkipToken({paramName}))");
            content.AppendLine("    {");
            content.AppendLine("        Logger.error('Skipping GetEPGFilePreviewById');");
            content.AppendLine("        return undefined;");
            content.AppendLine("    }");
        }

        content.AppendLine($"    Logger.debug('Fetching {method.Name}');");
        content.AppendLine("    const fetchDebug = localStorage.getItem('fetchDebug');");
        content.AppendLine("    const start = performance.now();");
        content.AppendLine($"    const response = await {method.Name}({paramName2});");
        content.AppendLine("    if (fetchDebug) {");
        content.AppendLine("      const duration = performance.now() - start;");
        content.AppendLine($"      Logger.debug(`Fetch {method.Name}" + " completed in ${duration.toFixed(2)}ms`);");
        content.AppendLine("    }");
        content.AppendLine("");
        //if (method.IsList)
        //{
        //    content.AppendLine($"    Logger.debug('Fetched {method.ProfileName} ',response?.length);");
        //}
        //else
        //{
        //    content.AppendLine($"    Logger.debug('Fetched {method.ProfileName}',response);");
        //}

        content.AppendLine($"    return {{param: {paramName}, value: response }};");
        content.AppendLine("  } catch (error) {");
        content.AppendLine("    console.error('Failed to fetch', error);");
        content.AppendLine("    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });");
        content.AppendLine("  }");
        content.AppendLine("});");
        return content.ToString();
    }

    private static string CreateGetPaged(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine($"export const fetch{method.Name} = createAsyncThunk('cache/get{method.Name}', async (query: string, thunkAPI) => {{");
        content.AppendLine("  try {");
        content.AppendLine("    if (isSkipToken(query))");
        content.AppendLine("    {");
        content.AppendLine("        Logger.error('Skipping GetEPGFilePreviewById');");
        content.AppendLine("        return undefined;");
        content.AppendLine("    }");
        content.AppendLine("    if (query === undefined) return;");
        //content.AppendLine($"    Logger.debug('Fetching {method.ProfileName}');");
        content.AppendLine("    const fetchDebug = localStorage.getItem('fetchDebug');");
        content.AppendLine("    const start = performance.now();");
        content.AppendLine("    const params = JSON.parse(query);");
        content.AppendLine($"    const response = await {method.Name}(params);");
        //content.AppendLine($"    Logger.debug('Fetched {method.ProfileName} ',response?.Data.length);");
        content.AppendLine("    if (fetchDebug) {");
        content.AppendLine("      const duration = performance.now() - start;");
        content.AppendLine($"      Logger.debug(`Fetch {method.Name}" + " completed in ${duration.toFixed(2)}ms`);");
        content.AppendLine("    }");
        content.AppendLine("    return { query: query, value: response };");
        content.AppendLine("  } catch (error) {");
        content.AppendLine("    console.error('Failed to fetch', error);");
        content.AppendLine("    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });");
        content.AppendLine("  }");
        content.AppendLine("});");
        return content.ToString();
    }
}
