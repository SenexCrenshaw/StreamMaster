using System.Text;

public static class TypeScriptCommandGenerator
{

    public static void GenerateFile(List<MethodDetails> methods, string filePath, string typePath)
    {
        StringBuilder content = new();

        content.Append(AddImports(methods));

        foreach (MethodDetails method in methods)
        {
            content.Append(AddMethod(method));
        }

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());

    }

    private static string AddMethod(MethodDetails method)
    {
        if (method.Name.StartsWith("CreateChannelGroup"))
        {
            int aaa = 1;
        }
        StringBuilder content = new();

        if (method.IsGetPaged)
        {
            method.ReturnType = method.ReturnType.Replace("APIResponse", "PagedResponse");
            content.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<{method.ReturnType} | undefined> => {{");
            content.AppendLine("  const signalRService = SignalRService.getInstance();");
            content.AppendLine($"  return await signalRService.invokeHubCommand<APIResponse<{method.ReturnEntityType}>>('{method.Name}', parameters)");
            content.AppendLine($"    .then((response) => {{");
            content.AppendLine("      if (response) {");
            content.AppendLine("        return response.pagedResponse;");
            content.AppendLine("      }");
            content.AppendLine("      return undefined;");
            content.AppendLine("    })");
            content.AppendLine("    .catch((error) => {");
            content.AppendLine("      console.error(error);");
            content.AppendLine("      return undefined;");
            content.AppendLine("    });");
        }
        else
        {


            if (method.Name.StartsWith("Get") && method.Name.EndsWith("Parameters"))
            {
                method.TsParameter = "Parameters: QueryStringParameters";
            }

            if (string.IsNullOrEmpty(method.TsParameter))
            {
                content.AppendLine($"export const {method.Name} = async (): Promise<{method.TsReturnType} | null> => {{");
                content.AppendLine("  const signalRService = SignalRService.getInstance();");
                content.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}');");
            }
            else
            {
                if (method.Parameter == "")
                {
                    content.AppendLine($"export const {method.Name} = async (): Promise<{method.TsReturnType} | null> => {{");
                    content.AppendLine("  const signalRService = SignalRService.getInstance();");
                    content.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}');");
                }
                else
                {
                    content.AppendLine($"export const {method.Name} = async (request: {method.TsParameter}): Promise<{method.TsReturnType} | null> => {{");
                    content.AppendLine("  const signalRService = SignalRService.getInstance();");
                    content.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}', request);");
                }

            }

        }

        content.AppendLine("};");
        content.AppendLine();
        return content.ToString();
    }

    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();

        //HashSet<string> includes = methods.Where(a => !a.IsGetPaged).SelectMany(x => new[] { x.ReturnEntityType, x.SingalRFunction }).ToHashSet();
        //HashSet<string> returns = methods.Where(a => a.IsGetPaged).Select(x => x.ReturnEntityType).ToHashSet();

        HashSet<string> includes = methods
        .Where(a => !a.IsGet)
        .SelectMany(x => new[] { x.ReturnEntityType, x.SingalRFunction })
        .Union(methods.Where(a => a.IsGet).Select(x => x.ReturnEntityType))
        .ToHashSet();

        List<string> imports = [];
        foreach (string inc in includes)
        {

            string? test = Util.IsTSGeneric(inc);
            if (!string.IsNullOrEmpty(test)) { imports.Add(inc); }
        }

        if (methods.Any(a => a.Name.Contains("GetPagedM3UFiles")))
        {
            int aa = 1;
            MethodDetails b = methods.First(a => a.Name == "GetPagedM3UFiles");
        }

        if (methods.Any(a => a.IsGetPaged))
        {
            imports.Add("APIResponse");
            imports.Add("PagedResponse");
            imports.Add("QueryStringParameters");
        }

        content.AppendLine("import SignalRService from '@lib/signalr/SignalRService';");
        content.AppendLine($"import {{ {string.Join(",", imports)} }} from '@lib/smAPI/smapiTypes';");
        content.AppendLine();
        return content.ToString();
    }
}
