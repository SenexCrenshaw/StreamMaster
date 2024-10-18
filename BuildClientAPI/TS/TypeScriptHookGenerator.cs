using System.Text;

namespace BuildClientAPI.TS;

public static class TypeScriptHookGenerator
{
    private static readonly string[] IgnoreSystemUp = ["GetSettings", "GetTaskIsRunning"];

    public static void GenerateFile(List<MethodDetails> methods, string path)
    {
        foreach (MethodDetails method in methods.Where(a => a.IsGet))
        {
            StringBuilder content = new();
            content.Append(AddImports(method));
            content.Append(GenerateInterfaces(method));

            if (method.IsGetPaged)
            {
                content.Append(GeneratePagedHookContent(method));
            }
            else if (method.IsGetCached)
            {
                content.Append(GenerateGetCachedHookContent(method));
            }
            else
            {
                content.Append(GenerateHookContent(method));
            }

            content.Append(GenerateFooterContent(method));

            string fileName = $"use{method.Name}.tsx";
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
            if (method.IsList && !ret.EndsWith("[]"))
            {
                ret += "[]";
            }

            content.AppendLine($"interface ExtendedQueryHookResult extends QueryHookResult<{ret} | undefined> {{}}");
        }

        content.AppendLine("interface Result extends ExtendedQueryHookResult {");
        content.AppendLine("  Clear: () => void;");
        content.AppendLine("  ClearByTag: (tag: string) => void;");
        content.AppendLine("  SetField: (fieldData: FieldData) => void;");
        content.AppendLine("  SetIsForced: (force: boolean) => void;");
        content.AppendLine("  SetIsLoading: (isLoading: boolean, query: string) => void;");
        content.AppendLine("}");

        return content.ToString();
    }

    private static string AddImports(MethodDetails method)
    {
        StringBuilder content = new();
        string fetchActionName = $"fetch{method.Name}";
        const string p = "QueryHookResult";

        content.AppendLine($"import {{ {p} }} from '@lib/apiDefs';");
        content.AppendLine("import store, { RootState } from '@lib/redux/store';");
        content.AppendLine("import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';");
        content.AppendLine($"import {{ clear, clearByTag, setField, setIsForced, setIsLoading }} from './{method.Name}Slice';");
        content.AppendLine("import { useCallback,useEffect } from 'react';");
        if (!IgnoreSystemUp.Contains(method.Name))
        {
            content.AppendLine("import { useSMContext } from '@lib/context/SMProvider';");
        }

        if (method.IsGet && (method.IsGetPaged || method.ParameterNames.Length != 0))
        {
            content.AppendLine("import { SkipToken } from '@reduxjs/toolkit/query';");
            content.AppendLine("import { getParameters } from '@lib/common/getParameters';");
        }
        content.AppendLine($"import {{ {fetchActionName} }} from './{method.Name}Fetch';");

        string? a = Utils.IsTSGeneric(method.ReturnEntityType);
        List<string> pList = [];
        if (!string.IsNullOrEmpty(a))
        {
            pList.Add(a);
        }

        if (method.IsGetPaged)
        {
            pList.Add("PagedResponse");
            pList.Add("QueryStringParameters");
        }
        else if (method.IsGet && !string.IsNullOrEmpty(method.TsParameter))
        {
            pList.Add(method.TsParameter);
        }

        content.AppendLine($"import {{FieldData, {string.Join(",", pList)} }} from '@lib/smAPI/smapiTypes';");
        content.AppendLine();
        return content.ToString();
    }

    private static string GenerateHeader(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine(GenerateForced(method));

        return content.ToString();
    }

    #region Selectors

    private static string GeneratePagedSelectors(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine("const selectData = (state: RootState) => {");
        content.AppendLine("    if (query === undefined) return undefined;");
        content.AppendLine($"    return state.{method.Name}.data[query] || undefined;");
        content.AppendLine("  };");
        content.AppendLine("const data = useAppSelector(selectData);");
        content.AppendLine();

        content.AppendLine("const selectError = (state: RootState) => {");
        content.AppendLine("    if (query === undefined) return undefined;");
        content.AppendLine($"    return state.{method.Name}.error[query] || undefined;");
        content.AppendLine("  };");
        content.AppendLine("const error = useAppSelector(selectError);");
        content.AppendLine();

        content.AppendLine("const selectIsError = (state: RootState) => {");
        content.AppendLine("    if (query === undefined) return false;");
        content.AppendLine($"    return state.{method.Name}.isError[query] || false;");
        content.AppendLine("  };");
        content.AppendLine("const isError = useAppSelector(selectIsError);");
        content.AppendLine();

        content.AppendLine("const selectIsLoading = (state: RootState) => {");
        content.AppendLine("    if (query === undefined) return false;");
        content.AppendLine($"    return state.{method.Name}.isLoading[query] || false;");
        content.AppendLine("  };");
        content.AppendLine("const isLoading = useAppSelector(selectIsLoading);");

        return content.ToString();
    }

    private static string GenerateCachedSelectors(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine("const selectData = (state: RootState) => {");
        content.AppendLine("    if (param === undefined) return undefined;");
        content.AppendLine($"    return state.{method.Name}.data[param] || undefined;");
        content.AppendLine("  };");
        content.AppendLine("const data = useAppSelector(selectData);");
        content.AppendLine();

        content.AppendLine("const selectError = (state: RootState) => {");
        content.AppendLine("    if (param === undefined) return undefined;");
        content.AppendLine($"    return state.{method.Name}.error[param] || undefined;");
        content.AppendLine("  };");
        content.AppendLine("const error = useAppSelector(selectError);");
        content.AppendLine();

        content.AppendLine("const selectIsError = (state: RootState) => {");
        content.AppendLine("    if (param === undefined) return false;");
        content.AppendLine($"    return state.{method.Name}.isError[param] || false;");
        content.AppendLine("  };");
        content.AppendLine("const isError = useAppSelector(selectIsError);");
        content.AppendLine();

        content.AppendLine("const selectIsLoading = (state: RootState) => {");
        content.AppendLine("    if (param === undefined) return false;");
        content.AppendLine($"    return state.{method.Name}.isLoading[param] || false;");
        content.AppendLine("  };");
        content.AppendLine("const isLoading = useAppSelector(selectIsLoading);");

        return content.ToString();
    }

    private static string GenerateSelectors(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine("const selectData = (state: RootState) => {");
        content.AppendLine($"    return state.{method.Name}.data;");
        content.AppendLine("  };");
        content.AppendLine("const data = useAppSelector(selectData);");
        content.AppendLine();

        content.AppendLine("const selectError = (state: RootState) => {");
        content.AppendLine($"    return state.{method.Name}.error;");
        content.AppendLine("  };");
        content.AppendLine("const error = useAppSelector(selectError);");
        content.AppendLine();

        content.AppendLine("const selectIsError = (state: RootState) => {");
        content.AppendLine($"    return state.{method.Name}.isError;");
        content.AppendLine("  };");
        content.AppendLine("const isError = useAppSelector(selectIsError);");
        content.AppendLine();

        content.AppendLine("const selectIsLoading = (state: RootState) => {");
        content.AppendLine($"    return state.{method.Name}.isLoading;");
        content.AppendLine("  };");
        content.AppendLine("const isLoading = useAppSelector(selectIsLoading);");

        return content.ToString();
    }

    #endregion Selectors

    #region Headers

    private static string GeneratePagedForced(MethodDetails method)
    {
        StringBuilder content = new();
        //string pName = method.IsGetPaged ? "query" : "param";

        content.AppendLine($"  const isForced = useAppSelector((state) => state.{method.Name}.isForced ?? false);");
        content.AppendLine();
        content.AppendLine("  const SetIsForced = useCallback(");
        content.AppendLine("    (forceRefresh: boolean): void => {");
        content.AppendLine("      dispatch(setIsForced({ force: forceRefresh }));");
        content.AppendLine("    },");
        content.AppendLine("    [dispatch]");
        content.AppendLine("  );");
        content.AppendLine("  const ClearByTag = useCallback(");
        content.AppendLine("    (tag: string): void => {");
        content.AppendLine("      dispatch(clearByTag({tag: tag }));");
        content.AppendLine("    },");
        content.AppendLine("    [dispatch]");
        content.AppendLine("  );");
        content.AppendLine("");

        return content.ToString();
    }

    private static string GenerateForced(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine($"  const isForced = useAppSelector((state) => state.{method.Name}.isForced ?? false);");

        content.AppendLine();
        content.AppendLine("  const SetIsForced = useCallback(");
        content.AppendLine("    (forceRefresh: boolean): void => {");

        content.AppendLine("      dispatch(setIsForced({ force: forceRefresh }));");
        content.AppendLine("    },");
        content.AppendLine("    [dispatch]");
        content.AppendLine("  );");

        content.AppendLine("  const ClearByTag = useCallback(");
        content.AppendLine("    (tag: string): void => {");

        content.AppendLine("      dispatch(clearByTag({tag: tag }));");
        content.AppendLine("    },");

        content.AppendLine("    [dispatch]");

        content.AppendLine("  );");
        content.AppendLine("");

        return content.ToString();
    }

    private static string GeneratePagedHeader(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine("  const query = getParameters(params);");
        content.AppendLine(GeneratePagedForced(method));

        return content.ToString();
    }

    private static string GenerateGetHeader(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine("  const param = getParameters(params);");
        content.AppendLine(GeneratePagedForced(method));

        return content.ToString();
    }

    #endregion Headers

    #region Hook Content

    private static string GenerateHookContent(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine($"const use{method.Name} = (): Result => {{");

        if (!IgnoreSystemUp.Contains(method.Name))
        {
            content.AppendLine("  const { isSystemReady } = useSMContext();");
        }

        content.AppendLine("  const dispatch = useAppDispatch();");

        content.AppendLine(GenerateHeader(method));

        content.AppendLine("const SetIsLoading = useCallback(");
        content.AppendLine("  (isLoading: boolean): void => {");
        content.AppendLine("    dispatch(setIsLoading({ isLoading: isLoading }));");
        content.AppendLine("  },");
        content.AppendLine("  [dispatch]");
        content.AppendLine(");");

        content.AppendLine(GenerateSelectors(method));

        content.AppendLine();

        content.AppendLine("  useEffect(() => {");
        content.AppendLine($"    const state = store.getState().{method.Name};");
        content.AppendLine("    if (data === undefined && state.isLoading !== true && state.isForced !== true) {");
        content.AppendLine("      SetIsForced(true);");
        content.AppendLine("    }");
        content.AppendLine("  }, [SetIsForced, data]);");
        content.AppendLine();
        return content.ToString();
    }

    private static string GenerateGetCachedHookContent(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine($"const use{method.Name} = (params?: {method.TsParameter} | undefined | SkipToken): Result => {{");

        if (!IgnoreSystemUp.Contains(method.Name))
        {
            content.AppendLine("  const { isSystemReady } = useSMContext();");
        }

        content.AppendLine("  const dispatch = useAppDispatch();");

        content.AppendLine(GenerateGetHeader(method));

        content.AppendLine("  const SetIsLoading = useCallback(");
        content.AppendLine("    (isLoading: boolean, param: string): void => {");
        content.AppendLine("      if (param === undefined) return;");
        content.AppendLine("      dispatch(setIsLoading({ isLoading: isLoading, param: param }));");
        content.AppendLine("    },");
        content.AppendLine("    [dispatch]");
        content.AppendLine("  );");
        content.AppendLine();

        content.AppendLine(GenerateCachedSelectors(method));

        content.AppendLine();

        content.AppendLine("useEffect(() => {");
        content.AppendLine("  if (param === undefined) return;");
        content.AppendLine($"  const state = store.getState().{method.Name};");
        content.AppendLine("  if (data === undefined && state.isLoading[param] !== true && state.isForced !== true) {");
        content.AppendLine("    SetIsForced(true);");
        content.AppendLine("  }");
        content.AppendLine("}, [data, param, SetIsForced]);");
        content.AppendLine();

        return content.ToString();
    }

    private static string GeneratePagedHookContent(MethodDetails method)
    {
        StringBuilder content = new();

        content.AppendLine($"const use{method.Name} = (params?: QueryStringParameters | undefined | SkipToken): Result => {{");

        if (!IgnoreSystemUp.Contains(method.Name))
        {
            content.AppendLine("  const { isSystemReady } = useSMContext();");
        }

        content.AppendLine("  const dispatch = useAppDispatch();");

        content.AppendLine(GeneratePagedHeader(method));

        content.AppendLine("  const SetIsLoading = useCallback(");
        content.AppendLine("    (isLoading: boolean, query: string): void => {");
        content.AppendLine("      dispatch(setIsLoading({ isLoading: isLoading, query: query }));");
        content.AppendLine("    },");
        content.AppendLine("    [dispatch]");
        content.AppendLine("  );");

        content.AppendLine();

        content.AppendLine(GeneratePagedSelectors(method));

        content.AppendLine();

        content.AppendLine("useEffect(() => {");
        content.AppendLine("  if (query === undefined) return;");
        content.AppendLine($"  const state = store.getState().{method.Name};");
        content.AppendLine("");
        content.AppendLine("  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {");
        content.AppendLine("    SetIsForced(true);");
        content.AppendLine("  }");
        content.AppendLine("}, [data, query, SetIsForced]);");
        content.AppendLine();
        return content.ToString();
    }

    #endregion Hook Content

    #region Footers

    private static string GenerateFooterContent(MethodDetails method)
    {
        StringBuilder content = new();
        //content.AppendLine(GenerateSelectors(method));

        //content.AppendLine();

        if (method.IsGetPaged)
        {
            content.AppendLine("useEffect(() => {");
            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("    if (!isSystemReady) return;");
            }

            content.AppendLine("    if (query === undefined) return;");
            content.AppendLine($"    const state = store.getState().{method.Name};");
            content.AppendLine("    if (state.isLoading[query]) return;");
            content.AppendLine("    if (data !== undefined && !isForced) return;");
            content.AppendLine();
            content.AppendLine("    SetIsLoading(true, query);");
            content.AppendLine($"    dispatch(fetch{method.Name}(query));");
            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("}, [data, dispatch, isForced, isSystemReady, query, SetIsLoading]);");
            }
            else
            {
                content.AppendLine("}, [data, dispatch, isForced, query, SetIsLoading]);");
            }
        }
        else if (method.IsGetCached)
        {
            content.AppendLine("useEffect(() => {");
            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("    if (!isSystemReady) return;");
            }
            content.AppendLine("  if (param === undefined) return;");
            content.AppendLine($"  const state = store.getState().{method.Name};");

            content.AppendLine("  if (params === undefined || param === undefined || param === '{}' ) return;");
            content.AppendLine("  if (state.isLoading[param]) return;");

            content.AppendLine("  if (data !== undefined && !isForced) return;");
            content.AppendLine();
            content.AppendLine("  SetIsLoading(true, param);");
            content.AppendLine($"  dispatch(fetch{method.Name}(params as {method.TsParameter}));");

            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("}, [SetIsLoading, data, dispatch, isForced, isSystemReady, param, params]);");
            }
            else
            {
                content.AppendLine("}, [SetIsLoading, data, dispatch, isForced, param, params]);");
            }
        }
        else
        {
            content.AppendLine("useEffect(() => {");
            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("    if (!isSystemReady) return;");
            }

            content.AppendLine($"  const state = store.getState().{method.Name};");

            content.AppendLine("  if (state.isLoading) return;");
            content.AppendLine("  if (data !== undefined && !isForced) return;");
            content.AppendLine("");

            content.AppendLine("  SetIsLoading(true);");
            content.AppendLine($"  dispatch(fetch{method.Name}());");

            if (!IgnoreSystemUp.Contains(method.Name))
            {
                content.AppendLine("}, [SetIsLoading, data, dispatch, isForced, isSystemReady]);");
            }
            else
            {
                content.AppendLine("}, [SetIsLoading, data, dispatch, isForced ]);");
            }
        }

        content.AppendLine("");
        content.AppendLine("const SetField = (fieldData: FieldData): void => {");
        content.AppendLine("  dispatch(setField({ fieldData: fieldData }));");
        content.AppendLine("};");
        content.AppendLine("");
        content.AppendLine("const Clear = (): void => {");
        content.AppendLine("  dispatch(clear());");
        content.AppendLine("};");
        content.AppendLine("");
        content.AppendLine("return {");
        content.AppendLine("  Clear,");
        content.AppendLine("  ClearByTag,");
        content.AppendLine("  data,");
        content.AppendLine("  error,");
        content.AppendLine("  isError,");
        content.AppendLine("  isLoading,");
        content.AppendLine("  SetField,");
        content.AppendLine("  SetIsForced,");
        content.AppendLine("  SetIsLoading");
        content.AppendLine("};");
        content.AppendLine("};");
        content.AppendLine("");
        content.AppendLine($"export default use{method.Name};");

        return content.ToString();
    }

    #endregion Footers
}