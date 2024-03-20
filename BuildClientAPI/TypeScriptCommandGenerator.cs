using System.Text;

public static class TypeScriptCommandGenerator
{
    public static void GenerateFile(string namespaceName, string mainEntityName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder imports = new();
        StringBuilder tsCommands = new();


        imports.AppendLine("import { invokeHubCommand } from '@lib/signalr/signalr';");
        imports.AppendLine();
        HashSet<string> additionalImports = [];
        foreach (MethodDetails method in methods)
        {
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
                if (method.Name == "DeleteSMChannels")
                {
                    int r1 = 1;
                }
                string tsReturnType = method.ReturnType == "DefaultAPIResponse" ? "DefaultAPIResponse | null" : "any | null";
                if (method.Name.EndsWith("Parameters"))
                {
                    method.TsParameters = "Parameters: QueryStringParameters";
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

                tsCommands.AppendLine($"export const {method.Name} = async ({method.TsParameters}): Promise<{tsReturnType}> => {{");
                tsCommands.AppendLine($"  return await invokeHubCommand<{method.ReturnType}>('{method.Name}', {method.ParameterNames});");

            }

            tsCommands.AppendLine("};");
            tsCommands.AppendLine();

        }
        string additionals = "QueryStringParameters";

        if (additionalImports.Count > 0)
        {
            additionals = string.Join(",", additionalImports);
        }
        imports.Insert(0, $"import {{APIResponse, PagedResponse, {additionals}, {mainEntityName} }} from '@lib/apiDefs';\n");

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, imports.ToString() + tsCommands.ToString());
    }
}
