using System.Text;
namespace BuildClientAPI.TS;
public static class TypeScriptCommandGenerator
{

    public static void GenerateFile(List<MethodDetails> methods, string filePath)
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

        StringBuilder content = new();

        if (method.IsGetPaged)
        {
            if (method.IsList)
            {
                method.ReturnType = $"PagedResponse<{method.ReturnType}>";
            }
            content.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<PagedResponse<{method.ReturnEntityType}> | undefined> => {{");
            content.AppendLine("  if (isSkipToken(parameters) || parameters === undefined) {");
            content.AppendLine("    return undefined;");
            content.AppendLine("  }");
            content.AppendLine("  const signalRService = SignalRService.getInstance();");
            content.AppendLine($"  return await signalRService.invokeHubCommand<PagedResponse<{method.ReturnEntityType}>>('{method.Name}', parameters);");
        }
        else
        {
            if (method.Name.StartsWith("Get") && method.Name.EndsWith("Parameters"))
            {
                method.TsParameter = "Parameters: QueryStringParameters";
            }

            if (string.IsNullOrEmpty(method.TsParameter))
            {
                content.AppendLine($"export const {method.Name} = async (): Promise<{method.TsReturnType} | undefined> => {{");
                content.AppendLine("  const signalRService = SignalRService.getInstance();");
                content.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}');");
            }
            else
            {
                content.AppendLine($"export const {method.Name} = async (request: {method.TsParameter}): Promise<{method.TsReturnType} | undefined> => {{");
                if (method.Name.StartsWith("Get"))
                {
                    content.AppendLine("  if ( request === undefined ) {");
                    content.AppendLine("    return undefined;");
                    content.AppendLine("  }");
                }
                content.AppendLine("  const signalRService = SignalRService.getInstance();");
                content.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}', request);");
            }

        }

        content.AppendLine("};");
        content.AppendLine();
        return content.ToString();
    }

    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();

        HashSet<string> includes = methods
        .Where(a => !a.IsGet)
        .SelectMany(x => new[] { x.ReturnEntityType, x.SingalRFunction })
        .Union(methods.Where(a => a.IsGet).Select(x => x.ReturnEntityType))
        .Union(methods.Where(a => a.IsGet && !a.IsGetPaged).Select(x => x.TsParameter))
        .ToHashSet();

        HashSet<string> imports = [];
        foreach (string inc in includes)
        {
            string? test = Utils.IsTSGeneric(inc);
            if (!string.IsNullOrEmpty(test)) { imports.Add(test); }
        }

        if (methods.Any(a => a.IsGetPaged))
        {
            imports.Add("APIResponse");
            imports.Add("PagedResponse");
            imports.Add("QueryStringParameters");
        }

        content.AppendLine("import { isSkipToken } from '@lib/common/isSkipToken';");
        content.AppendLine("import SignalRService from '@lib/signalr/SignalRService';");
        content.AppendLine($"import {{ {string.Join(",", imports)} }} from '@lib/smAPI/smapiTypes';");
        content.AppendLine();
        return content.ToString();
    }
}
