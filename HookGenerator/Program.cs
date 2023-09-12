using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

internal class Program
{
    private const string SwaggerUrl = "http://127.0.0.1:7095/swagger/v1/swagger.json";
    private const string LocalFileName = "swagger.json";
    private const string OutputFileName = @"..\..\..\..\StreamMasterwebui\src\hooks\streammasterSignalrHooks.tsx";

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
                        string functionName = ConvertToTypeScriptPascalCase(operationId[(operationId.IndexOf('_') + 1)..]);

                        string? argType = GetArgType(method.Value);
                        if (argType != null)
                        {
                            argType = "iptv." + argType;
                        }

                        string responseType = GetResponseType(method.Value);
                        if (responseType != "void")
                        {
                            responseType = "iptv." + responseType;
                        }

                        tsContent.AppendLine();
                        tsContent.AppendLine($"export const {functionName} = async {(argType != null ? $"(arg: {argType})" : "()")}: Promise<{responseType}> => {{");
                        if (responseType != "void")
                        {
                            tsContent.AppendLine($"  const data = await hubConnection.invoke('{functionName}', {(argType != null ? "arg" : "")});");
                            tsContent.AppendLine($"  return data;");
                        }
                        else
                        {
                            tsContent.AppendLine($"  await hubConnection.invoke('{functionName}', {(argType != null ? "arg" : "")});");
                        }
                        tsContent.AppendLine("};");
                    }
                }
            }
        }

        File.WriteAllText(OutputFileName, tsContent.ToString());
    }

    public static string GetArgType(JsonElement methodElement)
    {
        if (methodElement.TryGetProperty("requestBody", out JsonElement requestBody) &&
            requestBody.TryGetProperty("content", out JsonElement content) &&
            content.TryGetProperty("application/json", out JsonElement jsonContent) &&
            jsonContent.TryGetProperty("schema", out JsonElement schema) &&
            schema.TryGetProperty("$ref", out JsonElement refElement))
        {
            return ConvertToTypeScriptPascalCase(refElement.GetString().Split("/")[^1]);
        }

        if (methodElement.TryGetProperty("parameters", out JsonElement parameters))
        {
            foreach (JsonElement parameter in parameters.EnumerateArray())
            {
                if (parameter.TryGetProperty("schema", out JsonElement parameterSchema) && parameterSchema.TryGetProperty("$ref", out JsonElement parameterRef))
                {
                    return ConvertToTypeScriptPascalCase(parameterRef.GetString().Split("/")[^1]);
                }
            }
        }
        return null;
    }

    public static string GetResponseType(JsonElement methodElement)
    {
        if (methodElement.TryGetProperty("responses", out JsonElement responses) &&
            responses.TryGetProperty("200", out JsonElement response200) &&
            response200.TryGetProperty("content", out JsonElement responseContent) &&
            responseContent.TryGetProperty("application/json", out JsonElement jsonResponse) &&
            jsonResponse.TryGetProperty("schema", out JsonElement responseSchema))
        {
            if (responseSchema.TryGetProperty("$ref", out JsonElement responseRef))
            {
                return ConvertToTypeScriptPascalCase(responseRef.GetString().Split("/")[^1]);
            }
            else if (responseSchema.TryGetProperty("type", out JsonElement typeProperty) &&
                     typeProperty.GetString() == "array" &&
                     responseSchema.TryGetProperty("items", out JsonElement items) &&
                     items.TryGetProperty("$ref", out JsonElement itemsRef))
            {
                return ConvertToTypeScriptPascalCase(itemsRef.GetString().Split("/")[^1]) + "[]";
            }
        }
        return "void";
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

    public static string ConvertToCamelCase(string pascalCaseString)
    {
        if (string.IsNullOrEmpty(pascalCaseString))
        {
            return pascalCaseString;
        }

        return char.ToLowerInvariant(pascalCaseString[0]) + pascalCaseString[1..];
    }
}
