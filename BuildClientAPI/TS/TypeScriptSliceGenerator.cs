using System.Text;

public static class TypeScriptSliceGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string path)
    {
        foreach (MethodDetails method in methods.Where(a => a.Name.StartsWith("Get")))
        {
            StringBuilder content = new();
            content.Append(AddImports(method));
            content.Append(GenerateQueryStateAndInitialState(method));

            // Define the slice
            content.AppendLine($"const {method.Name.ToCamelCase()}Slice = createSlice({{");
            content.AppendLine("  initialState,");
            content.AppendLine($"  name: '{method.Name}',");
            content.AppendLine("  reducers: {");
            content.AppendLine(GenerateActions(method));
            content.AppendLine("  extraReducers: (builder) => {");

            content.AppendLine(GenerateExtraReducerForFetch(method));
            content.AppendLine("  }");


            content.AppendLine("});");
            content.AppendLine();

            // Export actions and reducer
            content.AppendLine($"export const {{ clear, setIsLoading, setIsForced, setField }} = {method.Name.ToCamelCase()}Slice.actions;");
            content.AppendLine($"export default {method.Name.ToCamelCase()}Slice.reducer;");

            string fileName = $"{method.Name}Slice.ts";
            string filePath = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(filePath, content.ToString());
        }
    }

    private static string AddImports(MethodDetails method)
    {
        StringBuilder content = new();
        content.AppendLine("import { PayloadAction, createSlice } from '@reduxjs/toolkit';");

        string? a = Utils.IsTSGeneric(method.ReturnEntityType);
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

        string fetchActionName = $"fetch{method.Name}";
        content.AppendLine($"import {{ {fetchActionName} }} from '@lib/smAPI/{method.NamespaceName}/{method.Name}Fetch';");

        if (method.IsGetPaged)
        {
            content.AppendLine("import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';");
        }
        else
        {
            if (method.IsGet && !method.IsGetCached)
            {
                content.AppendLine("import { updateFieldInData } from '@lib/redux/updateFieldInData';");
            }
        }

        content.AppendLine();
        return content.ToString();
    }

    private static string GenerateQueryStateAndInitialState(MethodDetails method)
    {
        if (method.IsGetPaged)
        {
            return $@"
interface QueryState {{
  data: Record<string, PagedResponse<{method.ReturnEntityType}> | undefined>;
  error: Record<string, string | undefined>;
  isError: Record<string, boolean>;
  isForced: boolean;
  isLoading: Record<string, boolean>;
}}

const initialState: QueryState = {{
  data: {{}},
  error: {{}},
  isError: {{}},
  isForced: false,
  isLoading: {{}}
}};

";
        }
        else if (method.IsGetCached)
        {
            return $@"
interface QueryState {{
  data: Record<string, {method.ReturnEntityType} | undefined>;
  error: Record<string, string | undefined>;
  isError: Record<string, boolean>;
  isForced: boolean;
  isLoading: Record<string, boolean>;
}}

const initialState: QueryState = {{
  data: {{}},
  error: {{}},
  isError: {{}},
  isForced: false,
  isLoading: {{}}
}};

";
        }

        string ret = method.ReturnEntityType;
        if (method.IsList && !ret.EndsWith("[]"))
        {
            ret += "[]";
        }

        return $@"
interface QueryState {{
  data: {ret} | undefined;
  error: string | undefined;
  isError: boolean;
  isForced: boolean;
  isLoading: boolean;
}}

const initialState: QueryState = {{
  data: undefined,
  error: undefined,
  isError: false,
  isForced: false,
  isLoading: false
}};

";
    }

    private static string GenerateActions(MethodDetails method)
    {
        StringBuilder content = new();
        if (method.IsGetPaged)
        {
            content.AppendLine($"    clear: (state) => {{");
            content.AppendLine("      state = initialState;");
            content.AppendLine($"      console.log('{method.Name} clear');");
            content.AppendLine("    },");

            content.AppendLine($"    setField: (state, action: PayloadAction<{{ query?: string | undefined; fieldData: FieldData }}>) => {{");
            content.AppendLine("      const { query, fieldData } = action.payload;");
            content.AppendLine();
            content.AppendLine("      if (query !== undefined) {");
            content.AppendLine("        if (state.data[query]) {");
            content.AppendLine("          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);");
            content.AppendLine("        }");
            content.AppendLine("        return;");
            content.AppendLine("      }");
            content.AppendLine();
            content.AppendLine("      for (const key in state.data) {");
            content.AppendLine("        if (state.data[key]) {");
            content.AppendLine("          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);");
            content.AppendLine("        }");
            content.AppendLine("      }");
            content.AppendLine($"      console.log('{method.Name} setField');");
            content.AppendLine("    },");

            content.AppendLine("    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {");
            content.AppendLine("      const { force } = action.payload;");
            content.AppendLine("      state.isForced = force;");
            content.AppendLine($"      console.log('{method.Name}  setIsForced ', force);");
            content.AppendLine("    },");

            content.AppendLine($"    setIsLoading: (state, action: PayloadAction<{{ query: string; isLoading: boolean }}>) => {{");
            content.AppendLine("      const { query, isLoading } = action.payload;");
            content.AppendLine("      if (query !== undefined) {");
            content.AppendLine("        state.isLoading[query] = isLoading;");
            content.AppendLine("      } else {");
            content.AppendLine("        for (const key in state.data) {");
            content.AppendLine("          state.isLoading[key] = action.payload.isLoading;");
            content.AppendLine("        }");
            content.AppendLine("      }");
            content.AppendLine($"      console.log('{method.Name} setIsLoading ', action.payload.isLoading);");
            content.AppendLine("    }");

            content.AppendLine("  },");
            return content.ToString();
        }
        else if (method.IsGetCached)
        {
            content.AppendLine($"    clear: (state) => {{");
            content.AppendLine("      state = initialState;");
            content.AppendLine($"       console.log('{method.Name} clear');");
            content.AppendLine("    },");

            content.AppendLine($"    setField: (state, action: PayloadAction<{{ fieldData: FieldData }}>) => {{");
            content.AppendLine("      const { fieldData } = action.payload;");
            content.AppendLine();

            content.AppendLine("      if (fieldData.Entity !== undefined && state.data[fieldData.Id]) {");
            content.AppendLine("        state.data[fieldData.Id] = fieldData.Value;");
            content.AppendLine("        return;");
            content.AppendLine("      }");

            content.AppendLine($"      console.log('{method.Name} setField');");
            content.AppendLine("    },");

            content.AppendLine("    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {");
            content.AppendLine("      const { force } = action.payload;");
            content.AppendLine("      state.isForced = force;");
            content.AppendLine($"      console.log('{method.Name}  setIsForced ', force);");
            content.AppendLine("    },");

            content.AppendLine($"    setIsLoading: (state, action: PayloadAction<{{ param: string; isLoading: boolean }}>) => {{");
            content.AppendLine("      const { param, isLoading } = action.payload;");
            content.AppendLine("      if (param !== undefined) {");
            content.AppendLine("        const paramString = JSON.stringify(param);");
            content.AppendLine("        state.isLoading[paramString] = isLoading;");
            content.AppendLine("      } else {");
            content.AppendLine("        for (const key in state.data) {");
            content.AppendLine("          state.isLoading[key] = action.payload.isLoading;");
            content.AppendLine("        }");
            content.AppendLine("      }");
            content.AppendLine($"      console.log('{method.Name} setIsLoading ', action.payload.isLoading);");
            content.AppendLine("    }");

            content.AppendLine("  },");
            return content.ToString();
        }

        content.AppendLine($"    clear: (state) => {{");
        content.AppendLine("      state = initialState;");
        content.AppendLine($"      console.log('{method.Name} clear');");
        content.AppendLine("    },");

        content.AppendLine($"    setField: (state, action: PayloadAction<{{ fieldData: FieldData }}>) => {{");
        content.AppendLine("      const { fieldData } = action.payload;");
        content.AppendLine("      state.data = updateFieldInData(state.data, fieldData);");
        content.AppendLine($"      console.log('{method.Name} setField');");
        content.AppendLine("    },");

        content.AppendLine("    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {");
        content.AppendLine("      const { force } = action.payload;");
        content.AppendLine("      state.isForced = force;");
        content.AppendLine($"      console.log('{method.Name}  setIsForced ', force);");
        content.AppendLine("    },");

        content.AppendLine($"    setIsLoading: (state, action: PayloadAction<{{isLoading: boolean }}>) => {{");
        content.AppendLine("      state.isLoading = action.payload.isLoading;");
        content.AppendLine($"      console.log('{method.Name} setIsLoading ', action.payload.isLoading);");
        content.AppendLine("    }");

        content.AppendLine("},");
        return content.ToString();
    }

    private static string GenerateExtraReducerForFetch(MethodDetails method)
    {
        string fetchActionName = $"fetch{method.Name}";
        StringBuilder content = new();
        if (method.IsGetPaged)
        {
            content.AppendLine("    builder");
            content.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
            content.AppendLine("        const query = action.meta.arg;");
            content.AppendLine("        state.isLoading[query] = true;");
            content.AppendLine("        state.isError[query] = false;");
            content.AppendLine("        state.isForced = false;");
            content.AppendLine("        state.error[query] = undefined;");
            content.AppendLine("      })");
            content.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
            content.AppendLine("        if (action.payload) {");
            content.AppendLine("          const { query, value } = action.payload;");
            content.AppendLine("          state.data[query] = value;");
            content.AppendLine("          state.isLoading[query] = false;");
            content.AppendLine("          state.isError[query] = false;");
            content.AppendLine("          state.error[query] = undefined;");
            content.AppendLine("          state.isForced = false;");
            content.AppendLine("        }");
            content.AppendLine("      })");
            content.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
            content.AppendLine("        const query = action.meta.arg;");
            content.AppendLine("        state.error[query] = action.error.message || 'Failed to fetch';");
            content.AppendLine("        state.isError[query] = true;");
            content.AppendLine("        state.isLoading[query] = false;");
            content.AppendLine("        state.isForced = false;");
            content.AppendLine("      });");
            return content.ToString();
        }
        else if (method.IsGetCached)
        {
            content.AppendLine("    builder");
            content.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
            content.AppendLine("        const paramString = JSON.stringify(action.meta.arg);");
            content.AppendLine("        state.isLoading[paramString] = true;");
            content.AppendLine("        state.isError[paramString] = false;");
            content.AppendLine("        state.isForced = false;");
            content.AppendLine("        state.error[paramString] = undefined;");
            content.AppendLine("      })");
            content.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
            content.AppendLine("        if (action.payload) {");
            content.AppendLine("          const { param, value } = action.payload;");
            content.AppendLine("          const paramString = JSON.stringify(param);");
            content.AppendLine("          state.data[paramString] = value;");
            content.AppendLine("          state.isLoading[paramString] = false;");
            content.AppendLine("          state.isError[paramString] = false;");
            content.AppendLine("          state.error[paramString] = undefined;");
            content.AppendLine("          state.isForced = false;");
            content.AppendLine("        }");
            content.AppendLine("      })");
            content.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
            content.AppendLine("        const paramString = JSON.stringify(action.meta.arg);");
            content.AppendLine("        state.error[paramString] = action.error.message || 'Failed to fetch';");
            content.AppendLine("        state.isError[paramString] = true;");
            content.AppendLine("        state.isLoading[paramString] = false;");
            content.AppendLine("        state.isForced = false;");
            content.AppendLine("      });");
            return content.ToString();
        }

        content.AppendLine("    builder");
        content.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
        content.AppendLine("        state.isLoading = true;");
        content.AppendLine("        state.isError = false;");
        content.AppendLine("        state.error = undefined;");
        content.AppendLine("        state.isForced = false;");
        content.AppendLine("      })");
        content.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
        content.AppendLine("        if (action.payload) {");
        content.AppendLine("          const { value } = action.payload;");
        content.AppendLine("          state.data = value ?? undefined;;");
        content.AppendLine("          state.isLoading = false;");
        content.AppendLine("          state.isError = false;");
        content.AppendLine("          state.error = undefined;");
        content.AppendLine("          state.isForced = false;");
        content.AppendLine("        }");
        content.AppendLine("      })");
        content.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
        content.AppendLine("        state.error = action.error.message || 'Failed to fetch';");
        content.AppendLine("        state.isError = true;");
        content.AppendLine("        state.isLoading = false;");
        content.AppendLine("        state.isForced = false;");
        content.AppendLine("      });");
        return content.ToString();
    }


}
