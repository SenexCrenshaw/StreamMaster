using StreamMasterApplication.Hubs;

using StreamMasterDomain.Attributes;

using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace signlar_function_builder;

internal partial class Program
{
    private static readonly List<EndPoint> EndPoints = new();

    private static readonly List<string> toIgnore = new() {"videoStreamsGetVideoStreams", "streamGroupsGetAllStatisticsForAllUrls", "programmesGetProgrammeNames", "videoStreamsGetVideoStream", "m3UStreamsGetM3UStream", "m3UStreamsGetM3UStreams", "m3UFilesGetM3UFile" };
    private static List<string> iptv = new();

    public static string GetName(string name)
    {
        try
        {
            string? param = iptv.SingleOrDefault(a => a.ToLower().Contains(name.ToLower() + ": build."));
            if (param == null)
            {
                Console.WriteLine($"Cannot find name for {name}");
                return "";
            }
            //param = param.Replace("export type ", "");
            param = param[..param.IndexOf(":")];
            return param.Trim();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static string GetReal(string name)
    {
        string? param = iptv.SingleOrDefault(a => a.ToLower().Contains("export type " + name.ToLower() + " "));
        if (param == null)
        {
            return "";
        }
        param = param.Replace("export type ", "");
        param = param[..param.IndexOf(" ")];
        return param;
    }

    private static void BuildEndpoints()
    {
        foreach (EndPoint ep in EndPoints.OrderBy(a => a.Getter))
        {
            Console.WriteLine($"{ep.NS} / ep.Getter: \"{ep.Getter}\" ep.DTONoArray: \"{ep.DTONoArray}\" {ep.DTOArray} {ep.HubBroadCast}");
        }

        string fileName = @"..\..\..\..\StreamMasterwebui\src\store\signlar\enhancedApi.tsx";
        StringBuilder towrite = new();
        _ = towrite.AppendLine("import { hubConnection } from '../../app/store';\r\n" +
                    "import * as StreamMasterApi from '../iptvApi';\r\n");

        _ = towrite.AppendLine("export const enhancedApi = StreamMasterApi.iptvApi.enhanceEndpoints({\r\n" +
                    " endpoints: {");

        foreach (EndPoint end in EndPoints.Where(a => a.Getter != "").OrderBy(a => a.Getter))
        {
            if (toIgnore.Contains(end.Getter))
            {
                continue;
            }
            if (end.DTONoArray.ToLower().Contains("streamingstatus"))
            {
            }

            if (end.JustUpdates)
            {
            }

            _ = towrite.AppendLine($"    {end.Getter}: {{\r\n      async onCacheEntryAdded(\r\n        arg,\r\n        {{ updateCachedData, cacheDataLoaded, cacheEntryRemoved }}\r\n      ) {{\r\n        try {{\r\n          await cacheDataLoaded;\r\n");

            if (!end.IsSingle)
            {
                _ = towrite.AppendLine(EndPointOutput.GetUpdates(end));
            }

            if (!end.JustUpdates)
            {
                _ = towrite.AppendLine(EndPointOutput.GetUpdate(end));
            }

            if (!end.IsSingle && !end.JustUpdates)
            {
                _ = towrite.AppendLine(EndPointOutput.GetDelete(end));
            }

            _ = towrite.AppendLine("\r\n                } catch {}\r\n\r\n                await cacheEntryRemoved;\r\n            }\r\n        },");

            Console.WriteLine(end.HubBroadCast);
        }

        _ = towrite.Append("    }\r\n});");

        File.WriteAllText(fileName, towrite.ToString());
    }

    private static void BuildEnums()
    {
        List<EnumDeff> enums = new();
        bool isEnum = false;
        string fileName = @"..\..\..\..\StreamMasterwebui\src\store\streammaster_enums.tsx";
        EnumDeff newEnum = new();

        foreach (FileInfo file in new DirectoryInfo(@"..\\..\\..\\..\\StreamMasterDomain\\Enums\\").EnumerateFiles())
        {
            foreach (string line in File.ReadLines(file.FullName))
            {
                string towork = line.Trim();

                if (towork.Contains('}'))
                {
                    isEnum = false;
                    continue;
                }
                if (towork.StartsWith("public enum"))
                {
                    newEnum = new EnumDeff { Name = towork[(towork.LastIndexOf(' ') + 1)..] };
                    isEnum = true;
                    enums.Add(newEnum);
                }

                if (!towork.Contains('=') || !isEnum)
                {
                    continue;
                }

                string key = towork[..towork.IndexOf(' ')];
                string value = towork[towork.LastIndexOf(' ')..];
                if (value.Contains(','))
                {
                    value = value.Remove(value.IndexOf(','));
                }
                newEnum.KeyValues[key] = value.Trim();
            }
        }
        using StreamWriter outputFile = new(fileName);

        foreach (EnumDeff e in enums.Where(a => a.KeyValues.Any()))
        {
            outputFile.WriteLine($"export enum {e.Name} {{");
            foreach (string key in e.KeyValues.Keys)
            {
                outputFile.WriteLine($"  {key} = {e.KeyValues[key]},");
            }
            outputFile.WriteLine($"}}\r\n");
        }
    }

    private static void BuildFunctions(MethodInfo[] myArrayMethodInfo)
    {
        List<string> imports = new();

        List<string> list = new();

        string fileName = @"..\..\..\..\StreamMasterwebui\src\store\signlar_functions.tsx";

        // Display information for all methods.
        for (int i = 0; i < myArrayMethodInfo.Length; i++)
        {
            MethodInfo method = myArrayMethodInfo[i];

            if (method.Name.ToLower().Contains("getstreamingstatus"))
            {
            }
            if (method.GetCustomAttribute<BuilderIgnore>() != null)
            {
                continue;
            }

            string returnValue = "void";
            string param = "";
            string name = method.Name;
            string arg = "";
            string ns = "";
            string prefix = "StreamMasterApi.";

            if (
                method.ReturnType.FullName is not null &&
                method.ReturnType.FullName.Contains("[[")
                )
            {
                returnValue = method.ReturnType.FullName[method.ReturnType.FullName.IndexOf("[[")..];

                //if (returnValue.ToLower().Contains("bool"))
                //{
                //    var adsads = 1;
                //}

                returnValue = returnValue.Substring(2, returnValue.IndexOf(", Version"));
                returnValue = returnValue[..returnValue.IndexOf(",")];
                //if (returnValue.Contains("Int32"))
                //{
                //    var sss = 1;
                //}

                bool isNullable = returnValue.StartsWith("System.Nullable");

                bool isArray = returnValue.Contains("[[") && !returnValue.Contains("Int32");

                ns = returnValue;
                if (isArray)
                {
                    ns = ns[(ns.IndexOf("[[") + 2)..];
                }
                ns = ns[(ns.LastIndexOf(".") + 1)..];

                if (isArray)
                {
                    returnValue = returnValue[(returnValue.IndexOf("[[") + 2)..];
                }

                returnValue = returnValue[(returnValue.LastIndexOf(".") + 1)..];

                string? testName = CheckValueType(returnValue);
                if (testName != null)
                {
                    returnValue = testName;
                    prefix = "";
                }
                else
                {
                    returnValue = GetReal(returnValue);
                    if (returnValue == "")
                    {
                        Debug.Assert(returnValue == "");
                    }
                    imports.Add("  type " + returnValue + ",");
                }

                if (isArray)
                {
                    returnValue += "[]";
                }
            }

            ParameterInfo[] pars = method.GetParameters();

            foreach (ParameterInfo p in pars)
            {
                if (
                    p.ParameterType.Name.ToLower().Contains("cancellationtoken") ||
                    p.ParameterType.Name.ToLower().Contains("dbset")
                    )
                {
                    continue;
                }

                arg = ",arg";
                string? testName = CheckValueType(p.ParameterType.Name);
                if (testName != null)
                {
                    param = "arg: " + testName;
                }
                else
                {
                    param = GetReal(p.ParameterType.Name);

                    if (param == "")
                    {
                        throw new Exception($"Cannot find {p.ParameterType.Name} for {name}");
                    }

                    imports.Add("  type " + param + ",");
                    if (name.ToLower() == param.ToLower())
                    {
                        name += "CMD";
                    }
                    param = "arg: " + param;
                }
            }

            if (ns.Contains('.'))
            {
                ns = ns[..ns.IndexOf(".")];
            }
            if (ns.Contains('['))
            {
                ns = ns[..ns.IndexOf("[")];
            }

            if (ns != "")
            {
                string hub = returnValue;
                if (hub.Contains('['))
                {
                    hub = hub[..hub.IndexOf("[")];
                }

                string broadcast = "BroadcastUpdate" + ns;
                string endpointName = GetName(name);
                string DTO = prefix + returnValue;

                if (endpointName != "")
                {
                    EndPoint? ep = EndPoints.FirstOrDefault(a => a.NS == ns && a.DTO == DTO);
                    if (ep == null)
                    {
                        ep = new EndPoint
                        {
                            NS = ns,
                            DTO = DTO,
                            HubBroadCast = broadcast
                        };
                        EndPoints.Add(ep);
                    }
                    else
                    {
                    }
                    if (name.StartsWith("Get"))
                    {
                        if (name.StartsWith("GetAllStatisticsForAllUrls"))
                        {
                        }
                        if (method.GetCustomAttribute<JustUpdates>() != null)
                        {
                            ep.JustUpdates = true;
                        }

                        if (returnValue.Contains('['))
                        {
                            Type test = method.ReturnType.GetTypeInfo().GetGenericArguments()[0].GetGenericArguments().Single();

                            foreach (PropertyInfo prop in test.GetProperties())
                            {
                                if (prop.GetCustomAttribute<SortBy>() != null)
                                {
                                    ep.SortBy = prop.Name.ToLower();
                                }
                                if (prop.GetCustomAttribute<IndexBy>() != null)
                                {
                                    IndexBy? attr = prop.GetCustomAttribute<IndexBy>();
                                    if (attr is not null)
                                        ep.IndexBy = attr.Value;// prop.Name.ToLower();
                                }
                            }

                            ep.Getter = endpointName;
                        }
                        else
                        {
                            if (method.GetCustomAttribute<JustUpdates>() != null)
                            {
                                ep.JustUpdates = true;
                                ep.IsSingle = true;
                            }

                            if (ep.Getter == "" || !returnValue.ToLower().EndsWith("dto"))
                            {
                                ep.NS = returnValue;
                                ep.Getter = endpointName;
                                ep.IsSingle = true;
                            }
                        }
                    }
                }
            }

            list.Add($"export const {name} = async ({param}): Promise<{returnValue}> => {{");

            if (returnValue != "void")
            {
                list.Add($"    const data = await hubConnection.invoke('{method.Name}'{arg});\n");
                list.Add($"    return data;");
            }
            else
            {
                list.Add($"    await hubConnection.invoke('{method.Name}'{arg});");
            }
            list.Add($"}};\n");
            //foreach (var line in list)
            //    Console.WriteLine(line);
        }

        List<string> towrite = new()
        {
            "import { hubConnection } from \"../app/store\";",
            "import {"
        };

        imports = imports.Distinct().Order().ToList();
        towrite.AddRange(imports);

        towrite.Add("} from \"./iptvApi\";\n");

        File.WriteAllLines(fileName, towrite);

        File.AppendAllLines(fileName, list);
    }

    private static string? CheckValueType(string name)
    {
        if (name.ToLower() == "int32")
        {
            return "number";
        }
        else if (name.ToLower() == "string")
        {
            return "string";
        }
        else if (name.ToLower() == "boolean")
        {
            return "boolean";
        }
        else if (name.ToLower() == "guid")
        {
            return "string";
        }
        return null;
    }

    private static void Main(string[] args)
    {
        iptv = File.ReadAllLines(@"..\..\..\..\StreamMasterwebui\src\store\iptvApi.ts").ToList();

        Type myType = typeof(StreamMasterHub);
        MethodInfo[] myArrayMethodInfo = myType.GetMethods(
            BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.DeclaredOnly
            );

        BuildFunctions(myArrayMethodInfo);
        BuildEndpoints();
        BuildEnums();
    }

    private class EnumDeff
    {
        public EnumDeff()
        {
        }

        public Dictionary<string, string> KeyValues { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }
}
