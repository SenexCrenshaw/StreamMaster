using System.Diagnostics;
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

        imports.AppendLine("import SignalRService from '@lib/signalr/SignalRService';");
        imports.AppendLine();
        HashSet<string> additionalImports = [];


        foreach (MethodDetails method in methods)
        {
            //string parameterLine = string.IsNullOrEmpty(method.TsParameters) ? "" : $"{method.Name}Request request";
            //string toSend = string.IsNullOrEmpty(method.Parameters) ? $"new {method.Name}()" : "request";

            if (method.IsGetPaged)
            {
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
            if (!string.IsNullOrEmpty(method.TsParameters) && !method.TsParameters.Contains("[]"))
            {
                if (method.TsParameters.Contains(","))
                {
                    string[] parts = method.TsParameters.Split(",");
                    string? test = Util.IsTSGeneric(parts[0]);
                    if (test != null)
                    {
                        additionalImports.Add(test);
                    }
                }
                else
                {
                    toImport = Util.IsTSGeneric(method.TsParameters);
                    if (toImport != null)
                    {

                        additionalImports.Add(toImport);
                    }
                }
            }

            toImport = Util.IsTSGeneric(Util.ExtractInnermostType(method.ReturnType));
            if (toImport != null)
            {

                additionalImports.Add(toImport);
            }

            if (method.IsGetPaged)
            {
                method.ReturnType = method.ReturnType.Replace("APIResponse", "PagedResponse");
                tsCommands.AppendLine($"export const {method.Name} = async (parameters: QueryStringParameters): Promise<{method.ReturnType} | undefined> => {{");
                tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<APIResponse<{toImport}>>('{method.Name}', parameters)");
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
                if (method.Name == "GetSettings")
                {

                }

                if (!string.IsNullOrEmpty(method.Parameters))
                {
                    typesImports.Add($"{method.Name}Request");

                }

                string tsReturnType = method.ReturnType == "DefaultAPIResponse" ? "DefaultAPIResponse | null" : "any | null";
                if (method.Name.EndsWith("Parameters"))
                {
                    method.TsParameters = "Parameters: QueryStringParameters";
                }

                if (string.IsNullOrEmpty(method.TsParameters))
                {
                    tsCommands.AppendLine($"export const {method.Name} = async (): Promise<{tsReturnType}> => {{");
                    tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                    tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<{method.ReturnType}>('{method.Name}');");
                }
                else
                {
                    tsCommands.AppendLine($"export const {method.Name} = async (request: {method.Name}Request): Promise<{tsReturnType}> => {{");
                    tsCommands.AppendLine("  const signalRService = SignalRService.getInstance();");
                    tsCommands.AppendLine($"  return await signalRService.invokeHubCommand<{method.ReturnType}>('{method.Name}', request);");
                }

            }

            tsCommands.AppendLine("};");
            tsCommands.AppendLine();

            if (method.IsGetPaged)
            {
                additionalImports.Add("APIResponse");
                additionalImports.Add("PagedResponse");
            }
        }



        if (additionalImports.Count > 0)
        {
            string additionals = string.Join(",", additionalImports);
            imports.Insert(0, $"import {{{additionals}}} from '@lib/apiDefs';\n");
        }


        if (typesImports.Count > 0)
        {
            string typesImportString = string.Join(",", typesImports);
            imports.AppendLine($"import {{ {typesImportString} }} from './{namespaceName}Types';");

        }


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

        if (tsTypesImports.Length > 0)
        {
            directory = Directory.GetParent(typePath).ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(typePath, tsTypesImports.ToString() + tsTypes.ToString());
        }

    }

    private static string BuildTSInterface(MethodDetails method)
    {
        StringBuilder imports = new();
        imports.AppendLine($"export interface {method.Name}Request {{");

        foreach (string pair in method.TsParameters.Split(","))
        {
            if (string.IsNullOrEmpty(pair))
            {
                return "";
            }
            try
            {
                string[] parts = pair.Split(":");
                string type = parts[1].Trim();
                string name = parts[0].Trim();
                imports.AppendLine($"  {name.ToCamelCase()}: {type};");
                string? a = Util.IsTSGeneric(type);
                if (a == "IFormFile")
                {
                    int aaa = 1;
                }
                if (a != null)
                {
                    string b = Util.MapCSharpTypeToTypeScript(a);
                    additionalIImports.Add(a);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        imports.AppendLine(" }");
        return imports.ToString();
    }
}
