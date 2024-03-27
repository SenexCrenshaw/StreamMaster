using System.Text;

public static class TypeScriptSliceGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath)
    {

        foreach (MethodDetails method in methods)
        {
            if (method.Name == "GetSettings")
            {
                int aa = 1;
            }
            string? mainEntityName = Util.IsTSGeneric(Util.ExtractInnermostType(method.ReturnType));
            StringBuilder content = new();

            // Add necessary imports
            AddImports(content, mainEntityName, method, namespaceName);

            // Generate QueryState and initialState
            content.Append(GenerateQueryStateAndInitialState(mainEntityName));

            // Define the slice
            content.AppendLine($"const {namespaceName}Slice = createSlice({{");
            content.AppendLine($"  name: '{namespaceName}',");
            content.AppendLine("  initialState,");
            content.AppendLine("  reducers: {");
            content.AppendLine(GenerateReducers(namespaceName));
            content.AppendLine("  },");
            content.AppendLine("  extraReducers: (builder) => {");
            // Assuming methods contain fetch actions
            //foreach (MethodDetails? method in methods.Where(m => m.JustHub))
            //{
            string fetchActionName = $"fetch{method.Name}";
            content.AppendLine(GenerateExtraReducerForFetch(fetchActionName, method.Name));
            //}
            content.AppendLine("  }");
            content.AppendLine("});");
            content.AppendLine();

            // Export actions and reducer
            content.AppendLine($"export const {{ intSet{namespaceName}IsLoading, clear{namespaceName}, update{namespaceName} }} = {namespaceName}Slice.actions;");
            content.AppendLine($"export default {namespaceName}Slice.reducer;");
            string directory = Directory.GetParent(filePath).ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, content.ToString());
        }
    }

    private static void AddImports(StringBuilder content, string mainEntityName, MethodDetails method, string namespaceName)
    {
        //string[] imports = methods.Select(a => ParameterConverter2.ExtractInnermostType(a.ReturnType)).ToArray();

        //string importsString = string.Join(",", imports);
        //string import = ParameterConverter2.ExtractInnermostType(method.ReturnType);

        content.AppendLine("import { PayloadAction, createSlice } from '@reduxjs/toolkit';");
        content.AppendLine($"import {{FieldData,PagedResponse, removeKeyFromData, {mainEntityName} }} from '@lib/apiDefs';");
        //foreach (MethodDetails? method in methods.Where(m => m.JustHub))
        //{
        //    string fetchActionName = $"fetch{method.Name}";
        //    content.AppendLine($"import {{ {fetchActionName} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Fetch';");
        //}

        string fetchActionName = $"fetch{method.Name}";
        content.AppendLine($"import {{ {fetchActionName} }} from '@lib/smAPI/{namespaceName}/{namespaceName}Fetch';");
        content.AppendLine("import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';");
        content.AppendLine();
    }

    private static string GenerateQueryStateAndInitialState(string mainEntityName)
    {
        return $@"
interface QueryState {{
  data: Record<string, PagedResponse<{mainEntityName}> | undefined>;
  isLoading: Record<string, boolean>;
  isError: Record<string, boolean>;
  error: Record<string, string>;
}}

const initialState: QueryState = {{
  data: {{}},
  isLoading: {{}},
  isError: {{}},
  error: {{}}
}};
";
    }

    private static string GenerateExtraReducerForFetch(string fetchActionName, string methodName)
    {
        StringBuilder sb = new();
        sb.AppendLine($"    builder");
        sb.AppendLine($"      .addCase({fetchActionName}.pending, (state, action) => {{");
        sb.AppendLine($"        const query = action.meta.arg;");
        sb.AppendLine($"        state.isLoading[query] = true;");
        sb.AppendLine($"        state.isError[query] = false; // Reset isError state on new fetch");
        sb.AppendLine($"      }})");
        sb.AppendLine($"      .addCase({fetchActionName}.fulfilled, (state, action) => {{");
        sb.AppendLine($"        if (action.payload) {{");
        sb.AppendLine($"          const {{ query, value }} = action.payload;");
        sb.AppendLine($"          state.data[query] = value;");
        sb.AppendLine($"          state.isLoading[query] = false;");
        sb.AppendLine($"          state.isError[query] = false;");
        sb.AppendLine($"        }}");
        sb.AppendLine($"      }})");
        sb.AppendLine($"      .addCase({fetchActionName}.rejected, (state, action) => {{");
        sb.AppendLine($"        const query = action.meta.arg;");
        sb.AppendLine($"        state.error[query] = action.error.message || 'Failed to fetch';");
        sb.AppendLine($"        state.isError[query] = true;");
        sb.AppendLine($"        state.isLoading[query] = false;");
        sb.AppendLine($"      }});");
        return sb.ToString();
    }

    private static string GenerateReducers(string namespaceName)
    {
        StringBuilder sb = new();

        // Example reducer for updating streams based on a query and fieldData
        sb.AppendLine($"    update{namespaceName}: (state, action: PayloadAction<{{ query?: string | undefined; fieldData: FieldData }}>) => {{");
        sb.AppendLine($"      const {{ query, fieldData }} = action.payload;");
        sb.AppendLine();
        sb.AppendLine($"      if (query !== undefined) {{");
        sb.AppendLine($"        // Update a specific query's data if it exists");
        sb.AppendLine($"        if (state.data[query]) {{");
        sb.AppendLine($"          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);");
        sb.AppendLine($"        }}");
        sb.AppendLine($"        return;");
        sb.AppendLine($"      }}");
        sb.AppendLine();
        sb.AppendLine($"      // Fallback: update all queries' data if query is undefined");
        sb.AppendLine($"      for (const key in state.data) {{");
        sb.AppendLine($"        if (state.data[key]) {{");
        sb.AppendLine($"          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);");
        sb.AppendLine($"        }}");
        sb.AppendLine($"      }}");
        sb.AppendLine($"      console.log('update{namespaceName} executed');");
        sb.AppendLine($"    }},");

        sb.AppendLine($"    clear{namespaceName}: (state) => {{");
        sb.AppendLine($"      for (const key in state.data) {{");
        sb.AppendLine($"        const updatedData = removeKeyFromData(state.data, key);");
        sb.AppendLine($"        state.data = updatedData;");
        sb.AppendLine($"      }}");
        sb.AppendLine($"      console.log('clear{namespaceName} executed');");
        sb.AppendLine($"    }},");

        sb.AppendLine($"    intSet{namespaceName}IsLoading: (state, action: PayloadAction<{{isLoading: boolean }}>) => {{");
        sb.AppendLine("       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }");
        sb.AppendLine($"      console.log('set{namespaceName}IsLoading executed');");
        sb.AppendLine($"    }},");

        return sb.ToString();
    }

}
