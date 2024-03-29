using System.Text;

public static class TypeScriptCommandGenerator
{
    //private static HashSet<string> additionalIImports = [];
    public static void GenerateFile(List<MethodDetails> methods, string namespaceName, string filePath, string typePath)
    {
        StringBuilder imports = new();
        StringBuilder tsCommands = new();
        StringBuilder tsTypes = new();
        StringBuilder tsTypesImports = new();
        HashSet<string> typesImports = [];


        //typesImports.Add("DefaultAPIResponse");

        HashSet<string> includes = methods.SelectMany(x => x.SMAPIImport).Distinct().ToHashSet();
        //additionalIImports = [];

        imports.AppendLine("import SignalRService from '@lib/signalr/SignalRService';");
        imports.AppendLine();
        HashSet<string> additionalImports = [];

        foreach (MethodDetails method in methods)
        {
            if (method.IsGetPaged)
            {
                additionalImports.Add("QueryStringParameters");
            }

            string? toImport = null;
            if (!string.IsNullOrEmpty(method.TsParameter) && !method.TsParameter.Contains("[]"))
            {
                if (method.TsParameter.Contains(","))
                {
                    string[] parts = method.TsParameter.Split(",");
                    string? test = Util.IsTSGeneric(parts[0]);
                    if (test != null)
                    {

                        additionalImports.Add(test);
                    }
                }
                else
                {
                    toImport = Util.IsTSGeneric(method.TsParameter);
                    if (toImport != null)
                    {
                        if (method.Name.StartsWith("DeleteSMChannels"))
                        {
                            int aaa = 1;
                        }
                        additionalImports.Add(toImport);
                    }
                }
            }

            if (method.Name.StartsWith("CreateChannelGroup"))
            {
                int aaa = 1;
            }

            if (method.IsGetPaged)
            {
                includes.Add("APIResponse");
                includes.Add("PagedResponse");
                includes.Add("QueryStringParameters");
                includes.Add(method.ReturnEntityType);
                method.ReturnType = method.ReturnType.Replace("APIResponse", "PagedResponse");
                tsCommands.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<{method.ReturnType} | undefined> => {{");
                tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<APIResponse<{method.ReturnEntityType}>>('{method.Name}', parameters)");
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
                if (!string.IsNullOrEmpty(method.TsReturnType))
                {

                    string? ret = Util.IsTSGeneric(method.TsParameter);
                    if (!string.IsNullOrEmpty(ret))
                    {
                        typesImports.Add(method.TsParameter);
                    }

                    ret = Util.IsTSGeneric(method.TsReturnType);
                    if (!string.IsNullOrEmpty(ret) && !method.IsList)
                    {
                        typesImports.Add(method.TsReturnType);
                    }

                }

                if (method.Name.StartsWith("Get") && method.Name.EndsWith("Parameters"))
                {
                    method.TsParameter = "Parameters: QueryStringParameters";
                }

                if (string.IsNullOrEmpty(method.TsParameter))
                {
                    tsCommands.AppendLine($"export const {method.Name} = async (): Promise<{method.TsReturnType} | null> => {{");
                    tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                    tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}');");
                }
                else
                {
                    if (method.Parameter == "")
                    {
                        tsCommands.AppendLine($"export const {method.Name} = async (): Promise<{method.TsReturnType} | null> => {{");
                        tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                        tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}');");
                    }
                    else
                    {
                        tsCommands.AppendLine($"export const {method.Name} = async (request: {method.TsParameter}): Promise<{method.TsReturnType} | null> => {{");
                        tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                        tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<{method.TsReturnType}>('{method.Name}', request);");
                    }

                }

            }

            tsCommands.AppendLine("};");
            tsCommands.AppendLine();
        }

        if (includes.Count > 0)
        {
            string additionals = string.Join(",", includes);
            imports.Insert(0, $"import {{{additionals}}} from '@lib/smAPI/smapiTypes';\n");
        }

        if (additionalImports.Count > 0 || typesImports.Count > 0)
        {
            string additionals = string.Join(",", typesImports);
            imports.Insert(0, $"import {{{additionals}}} from '@lib/smAPI/smapiTypes';\n");
        }

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, imports.ToString() + tsCommands.ToString());

    }
}
