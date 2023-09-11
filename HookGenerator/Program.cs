using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

internal class Program
{
    private const string SwaggerUrl = "http://127.0.0.1:7095/swagger/v1/swagger.json";
    private const string LocalFileName = "swagger.json";

    private static async System.Threading.Tasks.Task Main(string[] args)
    {
        string swaggerJson;

        if (File.Exists(LocalFileName))
        {
            // Read from the local file if it exists
            swaggerJson = File.ReadAllText(LocalFileName);
        }
        else
        {
            // Otherwise, download the JSON and save it to a file
            using HttpClient httpClient = new();
            swaggerJson = await httpClient.GetStringAsync(SwaggerUrl);
            File.WriteAllText(LocalFileName, swaggerJson);
        }

        using JsonDocument doc = JsonDocument.Parse(swaggerJson);
        JsonElement root = doc.RootElement;

        StringBuilder tsContent = new();

        // Add imports
        tsContent.AppendLine("import { hubConnection } from \"../app/signalr\";");
        tsContent.AppendLine("import type * as iptv from \"../store/iptvApi\";\r\n");

        if (root.TryGetProperty("paths", out JsonElement paths))
        {
            foreach (JsonProperty path in paths.EnumerateObject())
            {
                foreach (JsonProperty method in path.Value.EnumerateObject())
                {
                    string operationId = method.Value.TryGetProperty("operationId", out JsonElement operationIdElement)
                        ? operationIdElement.GetString()
                        : null;

                    if (operationId != null)
                    {
                        string methodName = ConvertToTypeScriptPascalCase(operationId[(operationId.IndexOf('_') + 1)..]);

                        string? argType = method.Value.TryGetProperty("requestBody", out JsonElement requestBody) && requestBody.TryGetProperty("content", out JsonElement content) && content.TryGetProperty("application/json", out JsonElement jsonContent) && jsonContent.TryGetProperty("schema", out JsonElement schema) && schema.TryGetProperty("$ref", out JsonElement refElement)
                            ? "iptv." + ConvertToTypeScriptPascalCase(refElement.GetString().Split("/")[^1])
                            : null;

                        // If argType is not found in requestBody, look in the parameters section
                        if (argType == null && method.Value.TryGetProperty("parameters", out JsonElement parameters))
                        {
                            foreach (JsonElement parameter in parameters.EnumerateArray())
                            {
                                if (parameter.TryGetProperty("schema", out JsonElement parameterSchema) && parameterSchema.TryGetProperty("$ref", out JsonElement parameterRef))
                                {
                                    argType = "iptv." + ConvertToTypeScriptPascalCase(parameterRef.GetString().Split("/")[^1]);
                                    break;
                                }
                            }
                        }

                        string responseType = method.Value.TryGetProperty("responses", out JsonElement responses) && responses.TryGetProperty("200", out JsonElement response200) && response200.TryGetProperty("content", out JsonElement responseContent) && responseContent.TryGetProperty("application/json", out JsonElement jsonResponse) && jsonResponse.TryGetProperty("schema", out JsonElement responseSchema) && responseSchema.TryGetProperty("$ref", out JsonElement responseRef)
                            ? "iptv." + ConvertToTypeScriptPascalCase(responseRef.GetString().Split("/")[^1])
                            : "void";

                        tsContent.AppendLine();
                        tsContent.AppendLine($"export const {methodName} = async {(argType != null ? $"(arg: {argType})" : "()")}: Promise<{responseType}> => {{");
                        if (responseType != "void")
                        {
                            tsContent.AppendLine($"  const data = await hubConnection.invoke('{methodName}', {(argType != null ? "arg" : "")});");
                            tsContent.AppendLine($"  return data;");
                        }
                        else
                        {
                            tsContent.AppendLine($"  await hubConnection.invoke('{methodName}', {(argType != null ? "arg" : "")});");
                        }
                        tsContent.AppendLine("};");
                    }
                }
            }
        }

        string fileName = @"..\..\..\..\StreamMasterwebui\src\hooks\streammasterSignalrHooks.tsx";
        File.WriteAllText(fileName, tsContent.ToString());
    }

    public static string ConvertToTypeScriptPascalCase(string csharpName)
    {
        // Handle special abbreviations
        csharpName = csharpName.Replace("EPG", "Epg");
        csharpName = csharpName.Replace("SDStatus", "SdStatus");
        csharpName = csharpName.Replace("SMFileTypes", "SmFileTypes");

        // General PascalCase conversion
        csharpName = Regex.Replace(csharpName, @"([A-Z]+)([A-Z][a-z])", m => m.Groups[1].Value.ToUpper() + m.Groups[2].Value);

        return csharpName;
    }

}
