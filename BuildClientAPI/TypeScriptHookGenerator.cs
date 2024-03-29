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
        content.AppendLine("import store from '@lib/redux/store';");
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
        StringBuilder content = new();

        if (method.IsGetPaged)
        {
            content.AppendLine($"const use{method.Name} = (params?: GetApiArgument | undefined): Result => {{");
            content.AppendLine("  const dispatch = useAppDispatch();");
            content.AppendLine("  const query = JSON.stringify(params);");
            content.AppendLine($"  const data = useAppSelector((state) => state.{method.Name}.data[query]);");
            content.AppendLine($"  const isLoading = useAppSelector((state) => state.{method.Name}.isLoading[query] ?? false);");
            content.AppendLine($"  const isError = useAppSelector((state) => state.{method.Name}.isError[query] ?? false);");
            content.AppendLine($"  const error = useAppSelector((state) => state.{method.Name}.error[query] ?? '');");
            content.AppendLine();

            content.AppendLine($"  useEffect(() => {{");
            content.AppendLine("    if (params === undefined || query === undefined) return;");
            content.AppendLine($"    const state = store.getState().{method.Name};");
            content.AppendLine($"    if (state.data[query] !== undefined || state.isLoading[query]) return;");
            content.AppendLine($"    dispatch({fetchActionName}(query));");
            content.AppendLine($"  }}, [data, dispatch, params, query]);");
            content.AppendLine();
        }
        else
        {
            content.AppendLine($"const use{method.Name} = (): Result => {{");
            content.AppendLine("  const dispatch = useAppDispatch();");
            content.AppendLine($"  const data = useAppSelector((state) => state.{method.Name}.data);");
            content.AppendLine($"  const isLoading = useAppSelector((state) => state.{method.Name}.isLoading ?? false);");
            content.AppendLine($"  const isError = useAppSelector((state) => state.{method.Name}.isError ?? false);");
            content.AppendLine($"  const error = useAppSelector((state) => state.{method.Name}.error ?? '');");
            content.AppendLine();

            content.AppendLine($"  useEffect(() => {{");
            content.AppendLine($"    const test = store.getState().{method.Name};");
            content.AppendLine($"    if (test.data !== undefined || test.isLoading) return;");
            content.AppendLine($"    dispatch({fetchActionName}());");
            content.AppendLine($"  }}, [data, dispatch]);");
            content.AppendLine();
        }
        content.AppendLine($"  const set{method.Name}Field = (fieldData: FieldData): void => {{");
        content.AppendLine($"    dispatch(update{method.Name}({{ fieldData: fieldData }}));");
        content.AppendLine($"  }};");
        content.AppendLine();

        content.AppendLine($"  const refresh{method.Name} = (): void => {{");
        content.AppendLine($"    dispatch(clear{method.Name}());");
        content.AppendLine($"  }};");
        content.AppendLine();

        content.AppendLine($"  const set{method.Name}IsLoading = (isLoading: boolean): void => {{");
        content.AppendLine($"    dispatch(intSet{method.Name}IsLoading( {{isLoading: isLoading}} ));");
        content.AppendLine($"  }};");
        content.AppendLine();

        content.Append($"  return {{ data, error, isError, isLoading");

        content.AppendLine($", refresh{method.Name}, set{method.Name}Field, set{method.Name}IsLoading }};");
        content.AppendLine("};");
        content.AppendLine();
        content.AppendLine($"export default use{method.Name};");

        return content.ToString();

    }

}
