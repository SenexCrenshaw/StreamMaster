using System.Text;

public static class TypeScriptHookGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string path)
    {

        foreach (MethodDetails method in methods.Where(a => a.Name.StartsWith("Get")))
        {
            StringBuilder content = new();
            AddImports(content, method);
            content.Append(GenerateInterfaces(method));
            content.Append(GenerateHookContent(method));

            string fileName = $"use{method.Name}.ts";
            string filePath = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(filePath, content.ToString());
        }
    }

    private static string GenerateInterfaces(MethodDetails method)
    {
        StringBuilder content = new();
        if (method.IsGetPaged)
        {
            content.AppendLine($"interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<{method.ReturnEntityType}> | undefined> {{}}");
        }
        else
        {
            string ret = method.ReturnEntityType;
            if (method.IsList)
            {
                ret += "[]";
            }

            content.AppendLine($"interface ExtendedQueryHookResult extends QueryHookResult<{ret} | undefined> {{}}");
        }

        content.AppendLine();
        content.AppendLine($"interface Result extends ExtendedQueryHookResult {{");
        content.AppendLine($"  set{method.Name}Field: (fieldData: FieldData) => void;");
        content.AppendLine($"  refresh{method.Name}: () => void;");
        content.AppendLine($"  set{method.Name}IsLoading: (isLoading: boolean) => void;");
        content.AppendLine("}");
        return content.ToString();
    }

    private static void AddImports(StringBuilder content, MethodDetails method)
    {
        string fetchActionName = $"fetch{method.Name}";
        string p = "QueryHookResult";
        if (method.IsGetPaged)
        {
            p += ",GetApiArgument";
        }
        content.AppendLine($"import {{ {p} }} from '@lib/apiDefs';");
        content.AppendLine("import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';");
        content.AppendLine($"import {{ clear{method.Name}, intSet{method.Name}IsLoading, update{method.Name} }} from './{method.Name}Slice';");
        content.AppendLine("import { useEffect } from 'react';");
        content.AppendLine($"import {{ {fetchActionName} }} from './{method.NamespaceName}Fetch';");

        string? a = Util.IsTSGeneric(method.ReturnEntityType);
        List<string> pList = [];
        if (!string.IsNullOrEmpty(a))
        {
            pList.Add(a);
        }

        if (method.IsGetPaged)
        {
            pList.Add("PagedResponse");
        }

        content.AppendLine($"import {{FieldData, {string.Join(",", pList)} }} from '@lib/smAPI/smapiTypes';");
        content.AppendLine();
    }

    private static string GenerateHookContent(MethodDetails method)
    {
        string fetchActionName = $"fetch{method.Name}";
        StringBuilder sb = new();

        if (method.IsGetPaged)
        {
            sb.AppendLine($"const use{method.Name} = (params?: GetApiArgument | undefined): Result => {{");
            sb.AppendLine("  const dispatch = useAppDispatch();");
            sb.AppendLine("  const query = JSON.stringify(params);");
            sb.AppendLine($"  const data = useAppSelector((state) => state.{method.Name}.data[query]);");
            sb.AppendLine($"  const isLoading = useAppSelector((state) => state.{method.Name}.isLoading[query] ?? false);");
            sb.AppendLine($"  const isError = useAppSelector((state) => state.{method.Name}.isError[query] ?? false);");
            sb.AppendLine($"  const error = useAppSelector((state) => state.{method.Name}.error[query] ?? '');");
            sb.AppendLine();

            sb.AppendLine($"  useEffect(() => {{");
            sb.AppendLine($"    if (params === undefined || data !== undefined) return;");
            sb.AppendLine($"    dispatch({fetchActionName}(query));");
            sb.AppendLine($"  }}, [data, dispatch, params, query]);");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine($"const use{method.Name} = (): Result => {{");
            sb.AppendLine("  const dispatch = useAppDispatch();");
            sb.AppendLine($"  const data = useAppSelector((state) => state.{method.Name}.data);");
            sb.AppendLine($"  const isLoading = useAppSelector((state) => state.{method.Name}.isLoading ?? false);");
            sb.AppendLine($"  const isError = useAppSelector((state) => state.{method.Name}.isError ?? false);");
            sb.AppendLine($"  const error = useAppSelector((state) => state.{method.Name}.error ?? '');");
            sb.AppendLine();

            sb.AppendLine($"  useEffect(() => {{");
            sb.AppendLine($"    if ( data !== undefined) return;");
            sb.AppendLine($"    dispatch({fetchActionName}());");
            sb.AppendLine($"  }}, [data, dispatch]);");
            sb.AppendLine();
        }
        sb.AppendLine($"  const set{method.Name}Field = (fieldData: FieldData): void => {{");
        sb.AppendLine($"    dispatch(update{method.Name}({{ fieldData: fieldData }}));");
        sb.AppendLine($"  }};");
        sb.AppendLine();

        sb.AppendLine($"  const refresh{method.Name} = (): void => {{");
        sb.AppendLine($"    dispatch(clear{method.Name}());");
        sb.AppendLine($"  }};");
        sb.AppendLine();

        sb.AppendLine($"  const set{method.Name}IsLoading = (isLoading: boolean): void => {{");
        sb.AppendLine($"    dispatch(intSet{method.Name}IsLoading( {{isLoading: isLoading}} ));");
        sb.AppendLine($"  }};");
        sb.AppendLine();

        sb.Append($"  return {{ data, error, isError, isLoading");

        sb.AppendLine($", refresh{method.Name}, set{method.Name}Field, set{method.Name}IsLoading }};");
        sb.AppendLine("};");
        sb.AppendLine();
        sb.AppendLine($"export default use{method.Name};");

        return sb.ToString();

    }

}
