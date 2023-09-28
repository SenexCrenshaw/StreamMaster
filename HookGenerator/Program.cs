using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
internal class Program
{
    private static readonly List<string> blackList = new() { "programmesGetProgramme", "programmesGetProgrammeChannels", "programmesGetProgrammes", "programmesGetProgrammeFromDisplayName", "schedulesDirectGetHeadends", "schedulesDirectGetSchedules", "schedulesDirectGetStations", "videoStreamsGetAllStatisticsForAllUrls", "streamGroupVideoStreamsGetStreamGroupVideoStreamIds" };
    private static readonly Dictionary<string, string> overRideArgs = new() { { "GetIconFromSource", "StringArg" } };
    private static readonly Dictionary<string, string> additionalImports = new() { { "Icons", "import { type StringArg } from \"../../components/selectors/BaseSelector\";" } };


    private const string SwaggerUrl = "http://127.0.0.1:7095/swagger/v1/swagger.json";
    private const string LocalFileName = "swagger.json";
    private const string OutputDir = @"..\..\..\..\StreamMasterwebui\lib\smAPI";
    private static readonly Dictionary<string, StringBuilder> tagToGetContentMap = new();
    private static readonly Dictionary<string, StringBuilder> tagToMutateContentMap = new();
    private static readonly Dictionary<string, Dictionary<string, string>> getMethodResponseTypes = new();
    private static readonly Dictionary<string, Dictionary<string, string>> getMethodArgTypes = new();
    private static readonly Dictionary<string, List<string>> tagToGetMethodsMap = new();


    private static async Task Main(string[] args)
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
    private static (Dictionary<string, StringBuilder> getMap, Dictionary<string, StringBuilder> mutateMap) ParseSwaggerJson(string swaggerJson)
    {
        using JsonDocument doc = JsonDocument.Parse(swaggerJson);
        JsonElement root = doc.RootElement;
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
                        string functionName = ConvertToTypeScriptPascalCase(operationId[(operationId.IndexOf('_') + 1)..]);
                        if (functionName.Equals("GetPagedChannelGroups"))
                        {
                            int aa = 1;
                        }

                        string tag = tags[0].GetString();

                        string? argType = GetArgType(method.Value);
                        string responseType = GetResponseType(method.Value);
                        StringBuilder contentToUse;
                        if (method.Name.ToLower() == "get")
                        {

                            if (!tagToGetContentMap.ContainsKey(tag))
                            {
                                tagToGetContentMap[tag] = new StringBuilder();
                                // Add imports at the start of each new file's content
                                tagToGetContentMap[tag].AppendLine("import { hubConnection } from '@/lib/signalr/signalr';");
                                tagToGetContentMap[tag].AppendLine("import { isDebug } from '@/lib/settings';");
                                tagToGetContentMap[tag].AppendLine("import type * as iptv from '@/lib/iptvApi';");
                                if (additionalImports.ContainsKey(tag))
                                {
                                    tagToGetContentMap[tag].AppendLine(additionalImports[tag]);
                                }
                                tagToGetContentMap[tag].AppendLine("\r\n");
                            }
                            contentToUse = tagToGetContentMap[tag];

                            if (!tagToGetMethodsMap.ContainsKey(tag))
                            {
                                tagToGetMethodsMap[tag] = new List<string>();
                            }
                            tagToGetMethodsMap[tag].Add(functionName);

                            if (!getMethodArgTypes.ContainsKey(tag))
                            {
                                getMethodArgTypes[tag] = new();
                            }
                            getMethodArgTypes[tag][functionName] = argType;

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
                                tagToMutateContentMap[tag].AppendLine("import { hubConnection } from '@/lib/signalr/signalr';");
                                tagToMutateContentMap[tag].AppendLine("import { isDebug } from '@/lib/settings';");
                                tagToMutateContentMap[tag].AppendLine("import type * as iptv from '@/lib/iptvApi';\r\n");
                            }
                            contentToUse = tagToMutateContentMap[tag];
                        }
                        if (overRideArgs.ContainsKey(functionName))
                        {
                            argType = overRideArgs[functionName];
                        }
                        contentToUse.AppendLine($"export const {functionName} = async {(argType != null ? $"(arg: {argType})" : "()")}: Promise<{responseType}> => {{");
                        contentToUse.AppendLine($"  if (isDebug) console.log('{functionName}');");
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
        WriteContentBasedOnTag(tagToGetContentMap, "GetAPI");
        WriteContentBasedOnTag(tagToMutateContentMap, "MutateAPI");
        Dictionary<string, StringBuilder>? tagToRtkContentMap = BuildEnhanced();
        WriteContentBasedOnTag(tagToRtkContentMap, "EnhancedAPI");
        return (tagToGetContentMap, tagToMutateContentMap);
    }
    private static bool IsPaged(string responseType)
    {
        return responseType.StartsWith("PagedResponse") || responseType.Contains(".PagedResponse") || responseType.Contains("GetPaged");
    }
    private static bool IsArray(string responseType)
    {
        return responseType.EndsWith("[]");
    }
    private static bool IsPagedOrIsArray(string responseType)
    {
        return IsPaged(responseType) || IsArray(responseType);
    }
    private static string GetUpdateFunction(string endpointName, string argType, string responseType, string tag)
    {

        List<string> args = getMethodArgTypes.SelectMany(kv => kv.Value.Keys).ToList();
        List<string> resps = getMethodResponseTypes.SelectMany(kv => kv.Value.Keys).ToList();
        StringBuilder ret = new();
        string data = IsPaged(responseType) ? "data.data" : "data";
        string draft = IsPaged(argType) ? "draft.data" : "draft";
        if (IsPagedOrIsArray(responseType))
        {
            ret.AppendLine($"            if (!data || isEmptyObject(data)) {{");
            ret.AppendLine($"              console.log('empty', data);");
            ret.AppendLine($"              dispatch(iptvApi.util.invalidateTags(['{tag}']));");
            ret.AppendLine($"              return;");
            ret.AppendLine($"            }}");
            ret.AppendLine();
            ret.AppendLine($"            updateCachedData(() => {{");
            //ret.AppendLine($"              console.log('updateCachedData', data);");
            ret.AppendLine($"              for (const {{ endpointName, originalArgs }} of iptvApi.util.selectInvalidatedBy(getState(), [{{ type: '{tag}' }}])) {{");
            ret.AppendLine($"                if (endpointName !== '{endpointName}') continue;");
            ret.AppendLine($"                  dispatch(");
            ret.AppendLine($"                    iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{");
            ret.AppendLine();
            ret.AppendLine($"                      if (isPagedTableDto(data)) {{");
            ret.AppendLine($"                      {data}.forEach(item => {{");
            ret.AppendLine($"                        const index = {draft}.findIndex(existingItem => existingItem.id === item.id);");
            ret.AppendLine($"                        if (index !== -1) {{");
            ret.AppendLine($"                          {draft}[index] = item;");
            ret.AppendLine($"                        }}");
            ret.AppendLine($"                        }});");
            ret.AppendLine();
            ret.AppendLine($"                        return draft;");
            ret.AppendLine($"                        }}");
            ret.AppendLine();
            ret.AppendLine($"                      {data}.forEach(item => {{");
            ret.AppendLine($"                        const index = {draft}.findIndex(existingItem => existingItem.id === item.id);");
            ret.AppendLine($"                        if (index !== -1) {{");
            ret.AppendLine($"                          {draft}[index] = item;");
            ret.AppendLine($"                        }}");
            ret.AppendLine($"                        }});");
            ret.AppendLine();
            ret.AppendLine($"                      return draft;");
            ret.AppendLine($"                     }})");
            ret.AppendLine($"                   )");
            ret.AppendLine($"                 }}");
            //ret.AppendLine($"               }});");
            //ret.AppendLine($"             }};");


            ret.AppendLine();
            return ret.ToString();
        }
        ret.AppendLine($"            updateCachedData(() => {{");
        ret.AppendLine($"              console.log('updateCachedData', data);");
        ret.AppendLine($"              for (const {{ endpointName, originalArgs }} of iptvApi.util.selectInvalidatedBy(getState(), [{{ type: '{tag}' }}])) {{");
        ret.AppendLine($"                if (endpointName !== '{endpointName}') continue;");
        ret.AppendLine($"                  dispatch(iptvApi.util.updateQueryData(endpointName, originalArgs, (draft) => {{");
        ret.AppendLine($"                    console.log('updateCachedData', data, draft);");
        ret.AppendLine($"                   }})");
        ret.AppendLine($"                   );");
        ret.AppendLine($"                 }}");
        ret.AppendLine();

        return ret.ToString();
    }
    private static Dictionary<string, StringBuilder> BuildEnhanced()
    {
        Dictionary<string, StringBuilder> tagToRtkContentMap = new();
        foreach (string tag in tagToGetMethodsMap.Keys)
        {
            string singleTon = $"singleton{tag}Listener";

            StringBuilder rtkContent = new();

            rtkContent.AppendLine($"import {{ {singleTon} }} from '@/lib/signalr/singletonListeners';");
            rtkContent.AppendLine($"import {{ isEmptyObject }} from '@/lib/common/common';");
            rtkContent.AppendLine($"import isPagedTableDto from '@/lib/common/isPagedTableDto';");
            rtkContent.AppendLine($"import {{ iptvApi }} from '@/lib/iptvApi';");
            rtkContent.AppendLine($"import type * as iptv from '@/lib/iptvApi';");
            rtkContent.AppendLine();
            rtkContent.AppendLine($"export const enhancedApi{ConvertToTypeScriptPascalCase(tag)} = iptvApi.enhanceEndpoints({{");
            rtkContent.AppendLine($"  endpoints: {{");
            bool anyToWrite = false;
            // For every GET method in this tag
            foreach (string getMethod in tagToGetMethodsMap[tag])
            {
                string cased = ConvertToTypeScriptPascalCase(tag + getMethod);
                string name = ConvertToCamelCase(cased);
                string doName = $"do{cased}Update";
                string arg = $"iptv.{cased}ApiResponse";

                if (blackList.Contains(name))
                {
                    continue;
                }
                if (getMethod.Equals("GetPagedChannelGroups"))
                {
                    int aa = 1;
                }
                string responseType = getMethodResponseTypes[tag][getMethod];
                if (responseType == "void")
                {
                    continue;
                }

                //arg = getMethodArgTypes[tag][getMethod];

                anyToWrite = true;
                string updateFunction = GetUpdateFunction(name, arg, responseType, tag);

                rtkContent.AppendLine($"    {name}: {{");
                rtkContent.AppendLine($"      async onCacheEntryAdded(api, {{ dispatch, getState, updateCachedData, cacheDataLoaded, cacheEntryRemoved }}) {{");
                rtkContent.AppendLine($"        try {{");
                rtkContent.AppendLine($"          await cacheDataLoaded;");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"          const updateCachedDataWithResults = (data: {responseType}) => {{");
                //rtkContent.AppendLine($"            updateCachedData((draft: {arg}) => {{");
                rtkContent.AppendLine(updateFunction);
                //rtkContent.AppendLine($"              return draft;");
                rtkContent.AppendLine($"            }});");
                rtkContent.AppendLine($"          }};");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"         {singleTon}.addListener(updateCachedDataWithResults);");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"        await cacheEntryRemoved;");
                rtkContent.AppendLine($"        {singleTon}.removeListener(updateCachedDataWithResults);");
                rtkContent.AppendLine();
                rtkContent.AppendLine($"        }} catch (error) {{");
                rtkContent.AppendLine($"          console.error('Error in onCacheEntryAdded:', error);");
                rtkContent.AppendLine($"        }}");
                rtkContent.AppendLine();

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

    public static string? GetArgType(JsonElement methodElement)
    {

        if (methodElement.TryGetProperty("requestBody", out JsonElement requestBody) &&
            requestBody.TryGetProperty("content", out JsonElement content) &&
            content.TryGetProperty("application/json", out JsonElement jsonContent) &&
            jsonContent.TryGetProperty("schema", out JsonElement schema) &&
            schema.TryGetProperty("$ref", out JsonElement refElement))
        {
            return "iptv." + ConvertToTypeScriptPascalCase(refElement.GetString().Split("/")[^1]);
        }

        string resp = GetResponseType(methodElement, true);
        if (resp != "void")
        {
            return resp;
        }

        if (methodElement.TryGetProperty("parameters", out JsonElement parameters))
        {
            foreach (JsonElement parameter in parameters.EnumerateArray())
            {
                if (parameter.TryGetProperty("schema", out JsonElement parameterSchema))
                {
                    if (parameterSchema.TryGetProperty("$ref", out JsonElement parameterRef))
                    {
                        return "iptv." + ConvertToTypeScriptPascalCase(parameterRef.GetString().Split("/")[^1]);
                    }

                    if (parameterSchema.TryGetProperty("type", out JsonElement parameterType))
                    {
                        switch (parameterType.GetString().ToLower())
                        {
                            case "string":
                                return "string";
                            case "integer":
                                return "number";
                            default:
                                break;
                        }

                        return ConvertToTypeScriptPascalCase(parameterType.GetString());
                    }
                }
            }
        }

        return null;
    }

    public static string GetResponseType(JsonElement methodElement, bool keepPaged = false)
    {
        if (methodElement.TryGetProperty("responses", out JsonElement responses) &&
            responses.TryGetProperty("200", out JsonElement response200) &&
            response200.TryGetProperty("content", out JsonElement responseContent) &&
            responseContent.TryGetProperty("application/json", out JsonElement jsonResponse) &&
            jsonResponse.TryGetProperty("schema", out JsonElement responseSchema))
        {
            if (responseSchema.TryGetProperty("$ref", out JsonElement responseRef))
            {
                if (!keepPaged && responseRef.ToString().Contains("/PagedResponseOf"))
                {
                    string resp = responseRef.ToString().Split("/")[^1];
                    resp = resp.Replace("PagedResponseOf", "");
                    return "iptv." + ConvertToTypeScriptPascalCase(resp) + "[]";
                }
                return "iptv." + ConvertToTypeScriptPascalCase(responseRef.GetString().Split("/")[^1]);
            }
            else if (responseSchema.TryGetProperty("type", out JsonElement typeProperty) &&
                     typeProperty.GetString() == "array" &&
                     responseSchema.TryGetProperty("items", out JsonElement items) &&
                     items.TryGetProperty("$ref", out JsonElement itemsRef))
            {
                return "iptv." + ConvertToTypeScriptPascalCase(itemsRef.GetString().Split("/")[^1]) + "[]";
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