using System.Text;
using System.Text.RegularExpressions;

public static class TypeScriptHookGenerator2
{
    private static HashSet<string> additionalImports = [];
    public static void GenerateFile(string namespaceName, string mainEntityName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        additionalImports = [];


        content.AppendLine("import { useEffect } from 'react';");
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
        content.AppendLine($"import {{ clear{namespaceName}, update{namespaceName} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Slice';");
        content.AppendLine();


        content.Append(GenerateHookContent(namespaceName, mainEntityName, fetchMethod?.Name, methods));

        string additionals = "QueryStringParameters";

        if (additionalImports.Count > 0)
        {
            additionals = string.Join(",", additionalImports);
        }
        content.Insert(0, $"import {{ FieldData, GetApiArgument, PagedResponse, QueryHookResult,{mainEntityName},{additionals} }} from '@lib/apiDefs';\n");

        // Write to file
        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string GenerateHookContent(string namespaceName, string mainEntityName, string fetchMethodName, List<MethodDetails> methods)
    {
        methods = methods.Where(m => m.IncludeInHub).ToList();
        List<string> commandMethodNames = methods.Select(m => m.Name).ToList();

        StringBuilder sb = new();

        sb.AppendLine($"interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<{mainEntityName}> | undefined> {{}}");
        sb.AppendLine();
        sb.AppendLine($"interface {mainEntityName}Result extends ExtendedQueryHookResult {{");

        foreach (MethodDetails method in methods)
        {
            string methodName = method.Name;
            if (methodName.StartsWith("GetPaged"))
            {
                continue;
            }

            if (methodName == "DeleteAllSMChannelsFromParameters")
            {
                int aa = 1;
            }
            string? toImport = ParameterConverter.IsTSGeneric(method.TsParameters);
            if (toImport != null)
            {
                additionalImports.Add(toImport);
            }
            toImport = ParameterConverter.IsTSGeneric(method.ReturnType);
            if (toImport != null)
            {
                additionalImports.Add(toImport);
            }

            //(string parameterName, string tsParameterString) = GetReal(method);
            sb.AppendLine($"  {methodName.ToCamelCase()}: ({method.TsParameters}) => Promise<DefaultAPIResponse | null>;");
        }

        sb.AppendLine("  set" + namespaceName + "Field: (fieldData: FieldData) => void;");
        sb.AppendLine("  refresh" + namespaceName + ": () => void;");
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

        // Refresh field action
        sb.AppendLine($"  const refresh{namespaceName} = (): void => {{");
        sb.AppendLine($"    dispatch(clear{namespaceName}());");
        sb.AppendLine($"  }};");
        sb.AppendLine();

        // Command methods
        foreach (MethodDetails method in methods)
        {
            string methodName = method.Name;
            if (methodName.StartsWith("GetPaged"))
            {
                continue;
            }
            if (methodName == "DeleteAllSMChannelsFromParameters")
            {
                int aa = 1;
            }
            //string tsParams = ParameterConverter.ConvertCSharpParametersToTypeScript(method.Parameters);
            //string[] parts = tsParams.Split(':');
            //string parameterName = parts[0].Trim();

            //string pattern = @"\b\w*Parameters(?!\:)\b";
            //tsParams = Regex.Replace(tsParams, pattern, "QueryStringParameters");
            //(string parameterName, string tsParameterString) = GetReal(method);

            sb.AppendLine($"  const {methodName.ToCamelCase()} = ({method.TsParameters}): Promise<DefaultAPIResponse | null> => {{");
            sb.AppendLine($"    return {methodName}({method.ParameterNames});");
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

        sb.AppendLine($", refresh{namespaceName}, set{namespaceName}Field }};");
        sb.AppendLine("};");
        sb.AppendLine();
        sb.AppendLine($"export default use{namespaceName};");

        return sb.ToString();
    }

    public static (string parameterName, string tsParameterString) GetReal(MethodDetails method)
    {
        string tsParams = ParameterConverter.ConvertCSharpParametersToTypeScript(method.Parameters);
        string[] parts = tsParams.Split(':');
        string parameterName = parts[0].Trim();

        string pattern = @"\b\w*Parameters(?!\:)\b";
        tsParams = Regex.Replace(tsParams, pattern, "QueryStringParameters");
        return (parameterName, tsParams);
    }
}
