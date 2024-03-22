using System.Text;

public static class TypeScriptCommandGenerator
{
    private static HashSet<string> additionalIImports = [];
    public static void GenerateFile(List<MethodDetails> methods, string namespaceName, string filePath, string typePath)
    {
        StringBuilder imports = new();
        StringBuilder tsCommands = new();
        StringBuilder tsTypes = new();
        StringBuilder tsTypesImports = new();
        List<string> typesImports = [];
        additionalIImports = [];

        imports.AppendLine("import { invokeHubCommand } from '@lib/signalr/signalr';");
        imports.AppendLine();
        HashSet<string> additionalImports = [];

        if (namespaceName == "SMStreams")
        {
            int aaa = 1;
        }

        bool isGet = false;
        foreach (MethodDetails method in methods)
        {
            if (method.Name.StartsWith("GetPaged"))
            {
                isGet = true;
                if (!additionalImports.Contains("QueryStringParameters"))
                {
                    additionalImports.Add("QueryStringParameters");
                    additionalIImports.Add("QueryStringParameters");
                }
            }

            tsTypes.AppendLine(BuildTSInterface(method));

            if (method.TsReturnInterface != "")
            {
                tsTypes.AppendLine(method.TsReturnInterface);
            }

            string? toImport = null;
            if (!method.TsParameters.Contains("[]"))
            {
                toImport = ParameterConverter.IsTSGeneric(method.TsParameters);
                if (toImport != null)
                {
                    if (toImport == "SMChannelRankRequest")
                    {
                        int aaa = 1;
                    }
                    additionalImports.Add(toImport);
                }
            }

            toImport = ParameterConverter.IsTSGeneric(ParameterConverter.ExtractInnermostType(method.ReturnType));
            if (toImport != null)
            {
                if (toImport == "SMChannelRankRequest")
                {
                    int aaa = 1;
                }
                additionalImports.Add(toImport);
            }

            if (method.IsGet)
            {
                method.ReturnType = method.ReturnType.Replace("APIResponse", "PagedResponse");
                tsCommands.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<{method.ReturnType} | undefined> => {{");
                tsCommands.AppendLine($"  return await invokeHubCommand<APIResponse<{toImport}>>('{method.Name}', parameters)");
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
                typesImports.Add($"{method.Name}Request");
                string tsReturnType = method.ReturnType == "DefaultAPIResponse" ? "DefaultAPIResponse | null" : "any | null";
                if (method.Name.EndsWith("Parameters"))
                {
                    method.TsParameters = "Parameters: QueryStringParameters";
                }

                tsCommands.AppendLine($"export const {method.Name} = async (request: {method.Name}Request): Promise<{tsReturnType}> => {{");
                tsCommands.AppendLine($"  return await invokeHubCommand<{method.ReturnType}>('{method.Name}', request);");

            }

            tsCommands.AppendLine("};");
            tsCommands.AppendLine();

        }

        if (isGet)
        {
            additionalImports.Add("APIResponse");
            additionalImports.Add("PagedResponse");
        }

        if (additionalImports.Count > 0)
        {
            string additionals = string.Join(",", additionalImports);
            imports.Insert(0, $"import {{{additionals}}} from '@lib/apiDefs';\n");
        }

        string typesImportString = string.Join(",", typesImports);
        imports.AppendLine($"import {{ {typesImportString} }} from './{namespaceName}Types';");


        if (additionalIImports.Count > 0)
        {
            string additionals = string.Join(",", additionalIImports);
            tsTypesImports.AppendLine($"import {{ {additionals} }} from '@lib/apiDefs';");
        }

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, imports.ToString() + tsCommands.ToString());

        directory = Directory.GetParent(typePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(typePath, tsTypesImports.ToString() + tsTypes.ToString());
    }

    private static string BuildTSInterface(MethodDetails method)
    {
        StringBuilder imports = new();
        imports.AppendLine($"export interface {method.Name}Request {{");
        foreach (string pair in method.TsParameters.Split(","))
        {
            string[] parts = pair.Split(":");
            string type = parts[1].Trim();
            string name = parts[0].Trim();
            imports.AppendLine($"  {name}: {type};");
            string? a = ParameterConverter.IsTSGeneric(type);
            if (a != null)
            {
                additionalIImports.Add(a);
            }

        }

        imports.AppendLine(" }");
        return imports.ToString();
    }
}
