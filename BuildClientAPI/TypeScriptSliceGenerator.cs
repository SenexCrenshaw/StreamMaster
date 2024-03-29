using System.Text;

public static class TypeScriptSliceGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string path)
    {
        foreach (MethodDetails method in methods.Where(a => a.Name.StartsWith("Get")))
        {
            StringBuilder content = new();
            AddImports(content, method);

            content.Append(GenerateQueryStateAndInitialState(method));

            // Define the slice
            content.AppendLine($"const {method.Name.ToCamelCase()}Slice = createSlice({{");
            content.AppendLine($"  name: '{method.Name}',");
            content.AppendLine("  initialState,");
            content.AppendLine("  reducers: {");
            content.AppendLine(GenerateReducers(method));
            content.AppendLine("  },");
            content.AppendLine("  extraReducers: (builder) => {");

            content.AppendLine(GenerateExtraReducerForFetch(method));
            content.AppendLine("  }");


            content.AppendLine("});");
            content.AppendLine();

            // Export actions and reducer
            content.AppendLine($"export const {{ clear{method.Name}, intSet{method.Name}IsLoading, update{method.Name} }} = {method.Name.ToCamelCase()}Slice.actions;");
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

    private static void AddImports(StringBuilder content, MethodDetails method)
    {
        content.AppendLine("import { PayloadAction, createSlice } from '@reduxjs/toolkit';");

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
        if (method.IsGetPaged)
        {
            content.AppendLine($"import {{removeKeyFromData}} from '@lib/apiDefs';");
        }

        string fetchActionName = $"fetch{method.Name}";
        content.AppendLine($"import {{ {fetchActionName} }} from '@lib/smAPI/{method.NamespaceName}/{method.NamespaceName}Fetch';");

        content.AppendLine("import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';");
        content.AppendLine();
    }

    private static string GenerateQueryStateAndInitialState(MethodDetails method)
    {
        if (method.IsGetPaged)
        {
            return $@"
interface QueryState {{
  data: Record<string, PagedResponse<{method.ReturnEntityType}> | undefined>;
  isLoading: Record<string, boolean>;
  isError: Record<string, boolean>;
  error: Record<string, string | undefined>;
}}

const initialState: QueryState = {{
  data: {{}},
  isLoading: {{}},
  isError: {{}},
  error: {{}}
}};
";
        }

        string ret = method.ReturnEntityType;
        if (method.IsList)
        {
            ret += "[]";
        }

        return $@"
interface QueryState {{
  data: {ret} | undefined;
  isLoading: boolean;
  isError: boolean;
  error: string | undefined;
}}

const initialState: QueryState = {{
  data: undefined,
  isLoading: false,
  isError: false,
  error: undefined
}};
";
    }

    private static string GenerateReducers(MethodDetails method)
    {
        StringBuilder sb = new();
        if (method.IsGetPaged)
        {
            sb.AppendLine($"    update{method.Name}: (state, action: PayloadAction<{{ query?: string | undefined; fieldData: FieldData }}>) => {{");
            sb.AppendLine("      const { query, fieldData } = action.payload;");
            sb.AppendLine();
            sb.AppendLine("      if (query !== undefined) {");
            sb.AppendLine("        if (state.data[query]) {");
            sb.AppendLine("          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);");
            sb.AppendLine("        }");
            sb.AppendLine("        return;");
            sb.AppendLine("      }");
            sb.AppendLine();
            sb.AppendLine("      for (const key in state.data) {");
            sb.AppendLine("        if (state.data[key]) {");
            sb.AppendLine("          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);");
            sb.AppendLine("        }");
            sb.AppendLine("      }");
            sb.AppendLine($"      console.log('update{method.Name} executed');");
            sb.AppendLine("    },");

            sb.AppendLine($"    clear{method.Name}: (state) => {{");
            sb.AppendLine("      for (const key in state.data) {");
            sb.AppendLine("        const updatedData = removeKeyFromData(state.data, key);");
            sb.AppendLine("        state.data = updatedData;");
            sb.AppendLine("      }");
            sb.AppendLine($"      console.log('clear{method.Name} executed');");
            sb.AppendLine("    },");

            sb.AppendLine($"    intSet{method.Name}IsLoading: (state, action: PayloadAction<{{isLoading: boolean }}>) => {{");
            sb.AppendLine("       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }");
            sb.AppendLine($"      console.log('set{method.Name}IsLoading executed');");
            sb.AppendLine("    },");
            return sb.ToString();
        }


        sb.AppendLine($"    update{method.Name}: (state, action: PayloadAction<{{ fieldData: FieldData }}>) => {{");
        sb.AppendLine("      const { fieldData } = action.payload;");
        sb.AppendLine("      state.data = updatePagedResponseFieldInData(state.data, fieldData);");
        sb.AppendLine($"      console.log('update{method.Name} executed');");
        sb.AppendLine("    },");

        sb.AppendLine($"    clear{method.Name}: (state) => {{");
        sb.AppendLine("      state.data = undefined;");
        sb.AppendLine($"      console.log('clear{method.Name} executed');");
        sb.AppendLine("    },");

        sb.AppendLine($"    intSet{method.Name}IsLoading: (state, action: PayloadAction<{{isLoading: boolean }}>) => {{");
        sb.AppendLine("       state.isLoading = action.payload.isLoading;");
        sb.AppendLine($"      console.log('set{method.Name}IsLoading executed');");
        sb.AppendLine("    },");
        return sb.ToString();
    }

    private static string GenerateExtraReducerForFetch(MethodDetails method)
    {
        string fetchActionName = $"fetch{method.Name}";
        StringBuilder sb = new();
        if (method.IsGetPaged)
        {
            sb.AppendLine("    builder");
            sb.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
            sb.AppendLine("        const query = action.meta.arg;");
            sb.AppendLine("        state.isLoading[query] = true;");
            sb.AppendLine("        state.isError[query] = false;");
            sb.AppendLine("        state.error[query] = undefined;");
            sb.AppendLine("      })");
            sb.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
            sb.AppendLine("        if (action.payload) {");
            sb.AppendLine("          const { query, value } = action.payload;");
            sb.AppendLine("          state.data[query] = value;");
            sb.AppendLine("          state.isLoading[query] = false;");
            sb.AppendLine("          state.isError[query] = false;");
            sb.AppendLine("          state.error[query] = undefined;");
            sb.AppendLine("        }");
            sb.AppendLine("      })");
            sb.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
            sb.AppendLine("        const query = action.meta.arg;");
            sb.AppendLine("        state.error[query] = action.error.message || 'Failed to fetch';");
            sb.AppendLine("        state.isError[query] = true;");
            sb.AppendLine("        state.isLoading[query] = false;");
            sb.AppendLine("      });");
            return sb.ToString();
        }

        sb.AppendLine("    builder");
        sb.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
        sb.AppendLine("        state.isLoading = true;");
        sb.AppendLine("        state.isError = false;");
        sb.AppendLine("        state.error = undefined;");
        sb.AppendLine("      })");
        sb.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
        sb.AppendLine("        if (action.payload) {");
        sb.AppendLine("          const { value } = action.payload;");
        sb.AppendLine("          state.data = value ?? undefined;;");
        sb.AppendLine("          state.isLoading = false;");
        sb.AppendLine("          state.isError = false;");
        sb.AppendLine("          state.error = undefined;");
        sb.AppendLine("        }");
        sb.AppendLine("      })");
        sb.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
        sb.AppendLine("        state.error = action.error.message || 'Failed to fetch';");
        sb.AppendLine("        state.isError = true;");
        sb.AppendLine("        state.isLoading = false;");
        sb.AppendLine("      });");
        return sb.ToString();
    }


}
