using System.Text;

public static class TypeScriptCommandGenerator
{
    public static void GenerateFile(string namespaceName, string mainEntityName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder imports = new();
        StringBuilder tsCommands = new();

        string needDefaultImport = methods.Any(a => !a.Name.StartsWith("GetPaged")) ? "DefaultAPIResponse," : "";

        // Common imports for all generated files
        imports.AppendLine($"import {{APIResponse,{needDefaultImport} PagedResponse, QueryStringParameters, {mainEntityName} }} from '@lib/apiDefs';");
        imports.AppendLine("import { invokeHubCommand } from '@lib/signalr/signalr';");
        imports.AppendLine();

        foreach (MethodDetails method in methods)
        {
            // Generate parameter string, assuming method.Parameters already in "type name" format
            string paramList = method.Parameters.Any() ? ParameterConverter.ConvertCSharpParametersToTypeScript(method.Parameters) : "";

            // Distinguish between return types to apply specific logic
            if (method.Name.StartsWith("GetPaged"))
            {
                method.ReturnType = method.ReturnType.Replace("APIResponse", "PagedResponse");
                tsCommands.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<{method.ReturnType} | undefined> => {{");
                tsCommands.AppendLine($"  return await invokeHubCommand<APIResponse<{mainEntityName}>>('{method.Name}', parameters)");
                tsCommands.AppendLine($"    .then((response) => {{");
                tsCommands.AppendLine("      if (response) {");
                tsCommands.AppendLine("        return response.pagedResponse;");
                tsCommands.AppendLine("      }");
                tsCommands.AppendLine("      return undefined;");
                tsCommands.AppendLine("    })");
                tsCommands.AppendLine("    .catch((error) => {");
                tsCommands.AppendLine("      console.error(error);");
                tsCommands.AppendLine("      return undefined;");
                tsCommands.AppendLine("    });");
            }
            else
            {
                {
                    string tsReturnType = method.ReturnType == "DefaultAPIResponse" ? "DefaultAPIResponse | null" : "any | null";
                    if (method.Name.EndsWith("Parameters"))
                    {
                        paramList = "Parameters: QueryStringParameters";
                    }
                    tsCommands.AppendLine($"export const {method.Name} = async ({method.TsParameters}): Promise<{tsReturnType}> => {{");
                    tsCommands.AppendLine($"  return await invokeHubCommand<{method.ReturnType}>('{method.Name}', {method.ParameterNames});");
                }
            }

            tsCommands.AppendLine("};");
            tsCommands.AppendLine();
        }

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, imports.ToString() + tsCommands.ToString());
    }
}
