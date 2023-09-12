using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

internal class Program
{
    private const string SwaggerUrl = "http://127.0.0.1:7095/swagger/v1/swagger.json";
    private const string LocalFileName = "swagger.json";
    private const string OutputDir = @"..\..\..\..\StreamMasterwebui\src\smAPI";

    private static async System.Threading.Tasks.Task Main(string[] args)
    {
        string swaggerJson = await DownloadSwaggerJson();
        File.WriteAllText(LocalFileName, swaggerJson);

        (Dictionary<string, StringBuilder> getMap, Dictionary<string, StringBuilder> mutateMap) contentMap = ParseSwaggerJson(swaggerJson);

        WriteFiles(contentMap);
    }

    private static async Task<string> DownloadSwaggerJson()
    {
        using HttpClient httpClient = new();
        return await httpClient.GetStringAsync(SwaggerUrl);
    }

    //private class Response
    //{
    //    public Dictionary<string, string> MethodResponseType { get; set; } = new Dictionary<string, string>();
    //}
    private static (Dictionary<string, StringBuilder> getMap, Dictionary<string, StringBuilder> mutateMap) ParseSwaggerJson(string swaggerJson)
    {
        // ... Parsing logic ...

        using JsonDocument doc = JsonDocument.Parse(swaggerJson);
        JsonElement root = doc.RootElement;


        Dictionary<string, StringBuilder> tagToGetContentMap = new();
        Dictionary<string, StringBuilder> tagToMutateContentMap = new();
        Dictionary<string, Dictionary<string, string>> getMethodResponseTypes = new();
        Dictionary<string, List<string>> tagToGetMethodsMap = new();
        if (root.TryGetProperty("paths", out JsonElement paths))
        {
            foreach (JsonProperty path in paths.EnumerateObject())
            {
                foreach (JsonProperty method in path.Value.EnumerateObject())
                {
                    string? operationId = method.Value.TryGetProperty("operationId", out JsonElement operationIdElement)
                        ? operationIdElement.GetString()
                        : null;

                    if (operationId != null && method.Value.TryGetProperty("tags", out JsonElement tags))
                    {
                        string tag = tags[0].GetString();
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

                        StringBuilder contentToUse;


                        if (method.Name.ToLower() == "get")
                        {

                            if (!tagToGetContentMap.ContainsKey(tag))
                            {
                                tagToGetContentMap[tag] = new StringBuilder();
                                // Add imports at the start of each new file's content
                                tagToGetContentMap[tag].AppendLine("import { hubConnection } from \"../../app/signalr\";");
                                tagToGetContentMap[tag].AppendLine("import type * as iptv from \"../../store/iptvApi\";\r\n");

                            }
                            contentToUse = tagToGetContentMap[tag];

                            if (!tagToGetMethodsMap.ContainsKey(tag))
                            {
                                tagToGetMethodsMap[tag] = new List<string>();
                            }
                            tagToGetMethodsMap[tag].Add(functionName);

                            if (!getMethodResponseTypes.ContainsKey(tag))
                            {
                                getMethodResponseTypes[tag] = new();
                            }
                            getMethodResponseTypes[tag][functionName] = responseType;
                        }
                        else
                        {
                            if (!tagToMutateContentMap.ContainsKey(tag))
                            {
                                tagToMutateContentMap[tag] = new StringBuilder();
                                // Add imports at the start of each new file's content
                                tagToMutateContentMap[tag].AppendLine("import { hubConnection } from \"../../app/signalr\";");
                                tagToMutateContentMap[tag].AppendLine("import type * as iptv from \"../../store/iptvApi\";\r\n");
                            }
                            contentToUse = tagToMutateContentMap[tag];
                        }

                        contentToUse.AppendLine($"export const {functionName} = async {(argType != null ? $"(arg: {argType})" : "()")}: Promise<{responseType}> => {{");
                        if (responseType != "void")
                        {
                            contentToUse.AppendLine($"  const data = await hubConnection.invoke('{functionName}'{(argType != null ? ", arg" : "")});");
                            contentToUse.AppendLine($"  return data;");
                        }
                        else
                        {
                            contentToUse.AppendLine($"  await hubConnection.invoke('{functionName}'{(argType != null ? ", arg" : "")});");
                        }
                        contentToUse.AppendLine("};\r\n");
                    }
                }
            }
        }

        // Write content to individual files based on tags and method types
        WriteContentBasedOnTag(tagToGetContentMap, "GetAPI");
        WriteContentBasedOnTag(tagToMutateContentMap, "MutateAPI");
        Dictionary<string, StringBuilder>? tagToRtkContentMap = BuildEnhanced(tagToGetContentMap, tagToGetMethodsMap, getMethodResponseTypes);
        WriteContentBasedOnTag(tagToRtkContentMap, "EnhancedAPI");
        return (tagToGetContentMap, tagToMutateContentMap);
    }

    private static string GetUpdateFunction(string responseType)
    {
        StringBuilder ret = new();
        string data = responseType.StartsWith("PagedResponse") ? "data.data" : "data";
        if (responseType.StartsWith("PagedResponse"))
        {
            ret.AppendLine($"              {data}.forEach(item => {{");
            ret.AppendLine($"                const index = {data}.findIndex(existingItem => existingItem.id === item.id);");
            ret.AppendLine($"                if (index !== -1) {{");
            ret.AppendLine($"                  {data}[index] = item;");
            ret.AppendLine($"                }}");
            ret.AppendLine($"              }});");
            return ret.ToString();
        }

        return "              draft=data";
    }
    private static Dictionary<string, StringBuilder> BuildEnhanced(Dictionary<string, StringBuilder> tagToGetContentMap, Dictionary<string, List<string>> tagToGetMethodsMap, Dictionary<string, Dictionary<string, string>> getMethodResponseTypes)
    {
        Dictionary<string, StringBuilder> tagToRtkContentMap = new();

        foreach (string tag in tagToGetMethodsMap.Keys)
        {
            StringBuilder rtkContent = new();

            rtkContent.AppendLine($"import {{ hubConnection }} from '../../app/signalr';");
            rtkContent.AppendLine($"import {{ isEmptyObject }} from '../../common/common';");
            rtkContent.AppendLine($"import {{ iptvApi }} from '../../store/iptvApi';");
            rtkContent.AppendLine($"import type * as iptv from '../../store/iptvApi';");

            rtkContent.AppendLine();
            rtkContent.AppendLine($"export const enhancedApi{ConvertToTypeScriptPascalCase(tag)} = iptvApi.enhanceEndpoints({{");
            rtkContent.AppendLine($"  endpoints: {{");

            bool anyToWrite = false;
            // For every GET method in this tag
            foreach (string getMethod in tagToGetMethodsMap[tag])
            {
                string responseType = getMethodResponseTypes[tag][getMethod];
                if (responseType == "void")
                {
                    continue;
                }
                anyToWrite = true;
                string updateFunction = GetUpdateFunction(responseType);
                string name = ConvertToCamelCase(ConvertToTypeScriptPascalCase(tag + getMethod));
                rtkContent.AppendLine($"    {name}: {{");
                rtkContent.AppendLine($"      async onCacheEntryAdded(api, {{ dispatch, updateCachedData, cacheDataLoaded, cacheEntryRemoved }}) {{");
                rtkContent.AppendLine($"        try {{");
                rtkContent.AppendLine($"          await cacheDataLoaded;");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"          const updateCachedDataWithResults = (data: {responseType}) => {{");
                rtkContent.AppendLine($"            updateCachedData((draft: {responseType}) => {{");
                rtkContent.AppendLine(updateFunction);
                rtkContent.AppendLine($"              return draft;");
                rtkContent.AppendLine($"            }});");
                rtkContent.AppendLine($"          }};");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"          hubConnection.on('{tag}Refresh', (data: {responseType}) => {{");
                rtkContent.AppendLine($"            if (isEmptyObject(data)) {{");
                rtkContent.AppendLine($"              dispatch(iptvApi.util.invalidateTags(['{tag}']));");
                rtkContent.AppendLine($"            }} else {{");
                rtkContent.AppendLine($"              updateCachedDataWithResults(data);");
                rtkContent.AppendLine($"            }}");
                rtkContent.AppendLine($"          }});");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"        }} catch (error) {{");
                rtkContent.AppendLine($"          console.error('Error in onCacheEntryAdded:', error);");
                rtkContent.AppendLine($"        }}");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"        await cacheEntryRemoved;");
                rtkContent.AppendLine($"        hubConnection.off('{tag}Refresh');");
                rtkContent.AppendLine($"      }}");
                rtkContent.AppendLine($"    }},");
            }

            if (anyToWrite)
            {
                rtkContent.AppendLine($"  }}");
                rtkContent.AppendLine($"}});");

                tagToRtkContentMap[tag] = rtkContent;
            }

        }
        return tagToRtkContentMap;
    }

    private static void WriteFiles((Dictionary<string, StringBuilder> getMap, Dictionary<string, StringBuilder> mutateMap) maps)
    {
        WriteContentBasedOnTag(maps.getMap, "GetAPI");
        WriteContentBasedOnTag(maps.mutateMap, "MutateAPI");
    }

    private static void WriteContentBasedOnTag(Dictionary<string, StringBuilder> tagToContentMap, string suffix)
    {
        foreach (KeyValuePair<string, StringBuilder> kvp in tagToContentMap)
        {
            string tagDir = Path.Combine(OutputDir, ConvertToTypeScriptPascalCase(kvp.Key));
            if (!Directory.Exists(tagDir))
            {
                Directory.CreateDirectory(tagDir);
            }
            string fileName = $"{ConvertToTypeScriptPascalCase(kvp.Key)}{suffix}.tsx";
            File.WriteAllText(Path.Combine(tagDir, fileName), kvp.Value.ToString());
        }
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