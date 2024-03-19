using System.Text;

public static class TypeScriptHookGenerator
{
    public static void GenerateFile(string namespaceName, string mainEntityName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        //string[] imports = methods.Select(a => ParameterConverter.ExtractInnermostType(a.ReturnType)).ToArray();

        //string importsString = string.Join(",", imports);
        // Step 1: Add necessary imports
        content.AppendLine("import { useEffect } from 'react';");
        content.AppendLine($"import {{ DefaultAPIResponse, FieldData, GetApiArgument, PagedResponse, QueryHookResult,{mainEntityName} }} from '@lib/apiDefs';");
        content.AppendLine("import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';");

        // Importing command and fetch methods
        foreach (MethodDetails method in methods)
        {
            if (method.Name.StartsWith("GetPaged"))
            {
                continue;
            }
            if (method.IncludeInHub)
            {
                content.AppendLine($"import {{ {method.Name} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Commands';");
            }
        }

        MethodDetails? fetchMethod = methods.FirstOrDefault(m => m.Name.Contains("GetPaged"));
        if (fetchMethod != null)
        {
            content.AppendLine($"import {{ fetch{fetchMethod.Name} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Fetch';");
        }

        // Importing slice for actions
        content.AppendLine($"import {{ update{namespaceName} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Slice';");
        content.AppendLine();

        // Step 2: Define the hook and its return type
        content.Append(GenerateHookContent(namespaceName, mainEntityName, fetchMethod?.Name, methods.Where(m => m.IncludeInHub).Select(m => m.Name).ToList()));

        // Write to file

        File.WriteAllText(filePath, content.ToString());
    }

    private static string GenerateHookContent(string namespaceName, string mainEntityName, string fetchMethodName, IEnumerable<string> commandMethodNames)
    {
        StringBuilder sb = new();

        sb.AppendLine($"interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<{mainEntityName}> | undefined> {{}}");
        sb.AppendLine();
        sb.AppendLine($"interface {mainEntityName}Result extends ExtendedQueryHookResult {{");

        foreach (string methodName in commandMethodNames)
        {
            if (methodName.StartsWith("GetPaged"))
            {
                continue;
            }
            sb.AppendLine($"  {methodName.ToCamelCase()}: (id: string) => Promise<DefaultAPIResponse | null>;");
        }

        sb.AppendLine("  set" + namespaceName + "Field: (fieldData: FieldData) => void;");
        sb.AppendLine("}");
        sb.AppendLine();

        // Generating the hook function
        sb.AppendLine($"const use{namespaceName} = (params?: GetApiArgument | undefined): {mainEntityName}Result => {{");
        sb.AppendLine("  const query = JSON.stringify(params);");
        sb.AppendLine("  const dispatch = useAppDispatch();");
        sb.AppendLine();
        sb.AppendLine($"  const data = useAppSelector((state) => state.{namespaceName}.data[query]);");
        sb.AppendLine($"  const isLoading = useAppSelector((state) => state.{namespaceName}.isLoading[query] ?? false);");
        sb.AppendLine($"  const isError = useAppSelector((state) => state.{namespaceName}.isError[query] ?? false);");
        sb.AppendLine($"  const error = useAppSelector((state) => state.{namespaceName}.error[query] ?? '');");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(fetchMethodName))
        {
            sb.AppendLine($"  useEffect(() => {{");
            sb.AppendLine($"    if (params === undefined || data !== undefined) return;");
            sb.AppendLine($"    dispatch(fetch{fetchMethodName}(query));");
            sb.AppendLine($"  }}, [data, dispatch, params, query]);");
            sb.AppendLine();
        }

        // Set field action
        sb.AppendLine($"  const set{namespaceName}Field = (fieldData: FieldData): void => {{");
        sb.AppendLine($"    dispatch(update{namespaceName}({{ fieldData: fieldData }}));");
        sb.AppendLine($"  }};");
        sb.AppendLine();

        // Command methods
        foreach (string methodName in commandMethodNames)
        {
            if (methodName.StartsWith("GetPaged"))
            {
                continue;
            }
            sb.AppendLine($"  const {methodName.ToCamelCase()} = (id: string): Promise<DefaultAPIResponse | null> => {{");
            sb.AppendLine($"    return {methodName}(id);");
            sb.AppendLine($"  }};");
            sb.AppendLine();
        }

        // Return statement
        sb.Append($"  return {{ data, error, isError, isLoading");

        foreach (string methodName in commandMethodNames)
        {
            if (methodName.StartsWith("GetPaged"))
            {
                continue;
            }
            sb.Append($", {methodName.ToCamelCase()}");
        }

        sb.AppendLine($", set{namespaceName}Field }};");
        sb.AppendLine("};");
        sb.AppendLine();
        sb.AppendLine($"export default use{namespaceName};");

        return sb.ToString();
    }
}

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        return !string.IsNullOrEmpty(str) && str.Length > 1 ? char.ToLowerInvariant(str[0]) + str[1..] : str;
    }
}
