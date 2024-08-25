using AutoMapper.Internal;

using BuildClientAPI.CSharp;
using BuildClientAPI.TS;

using MediatR;

using StreamMaster.Domain.Attributes;

using System.Reflection;

namespace BuildClientAPI
{
    internal static class Program
    {
        private const string AssemblyName = "StreamMaster.Application";
        private const string CSharpFileNamePrefix = @"..\..\..\..\StreamMaster.Application\";
        private const string SMAPIFileNamePrefix = @"..\..\..\..\streammasterwebui\lib\smAPI";
        private const string SignalRFilePathPrefix = @"..\..\..\..\streammasterwebui\lib\signalr";
        private const string StoreFilePathPrefix = @"..\..\..\..\streammasterwebui\lib\redux";
        private const string DataRefreshFilePath = @"..\..\..\..\StreamMaster.Infrastructure\Services\DataRefreshService.cs";
        private const string DataRefreshAllFilePath = SMAPIFileNamePrefix + @"\DataRefreshAll.ts";
        private const string IDataRefreshFilePath = @"..\..\..\..\StreamMaster.Domain\Services\IDataRefreshService.cs";

        private static void Main(string[] args)
        {
            CleanSubDirectories(SMAPIFileNamePrefix);
            if (File.Exists(DataRefreshAllFilePath))
            {
                File.Delete(DataRefreshAllFilePath);
            }
            ScanForSMAPI();
        }

        private static void CleanSubDirectories(string path)
        {
            if (Directory.Exists(path))
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    Directory.Delete(dir, true);
                }
            }
            else
            {
                _ = Directory.CreateDirectory(path);
            }
        }

        private static void ScanForSMAPI()
        {
            bool writeFiles = true;
            try
            {
                Assembly assembly = Assembly.Load(AssemblyName);
                Dictionary<string, List<MethodDetails>> methodsByNamespace = [];
                List<Type> smapiAttributedTypes = assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(SMAPIAttribute), false).Any())
                    .ToList();

                foreach (Type recordType in smapiAttributedTypes)
                {
                    if (!recordType.Name.EndsWith("Request"))
                    {
                        continue;
                    }

                    string classNamespace = GetNameSpaceParent(recordType);
                    if (!methodsByNamespace.TryGetValue(classNamespace, out List<MethodDetails>? methodDetailsList))
                    {
                        methodDetailsList = [];
                        methodsByNamespace[classNamespace] = methodDetailsList;

                    }

                    string ps = CSharpUtils.ParamsToCSharp(recordType);
                    string tsps = Utils.CSharpParamToTS(recordType);

                    ConstructorInfo[] constructors = recordType.GetConstructors();
                    ParameterInfo[] parameters = constructors[0].GetParameters();

                    Type? iRequestInterface = recordType.GetInterfaces()
                                                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

                    if (iRequestInterface == null)
                    {
                        continue;
                    }

                    Type returnType = iRequestInterface.GenericTypeArguments[0];

                    List<string> smapiImport = [];

                    string toCheck = "SendSMTaskRequest";

                    if (recordType.Name.StartsWith(toCheck))
                    {
                        bool IsList = returnType.IsArray || returnType.IsListType() || returnType.IsDataResponse();
                        string cleanReturnType = GetCleanReturnType(returnType);
                        string returnTypeTS = GetCleanTSReturnType(returnType);
                        string Parameters = CSharpUtils.ParamsToCSharp(recordType);
                        string TsParameters = Utils.CSharpParamToTS(recordType);
                        string TsReturnType = GetCleanTSReturnType(returnType);
                        string genericArgs = string.Join(", ", recordType.GetGenericArguments().Select(FormatTypeName));
                        string genericArgs2 = string.Join(", ", returnType.GetGenericArguments().Select(FormatTypeName));

                    }


                    string name = recordType.Name;
                    string parameter = recordType.Name;
                    if (recordType.Name.EndsWith("Request"))
                    {
                        name = recordType.Name[..^7];
                    }

                    SMAPIAttribute? smapiAttribute = recordType.GetCustomAttribute<SMAPIAttribute>();
                    if (smapiAttribute == null)
                    {
                        continue;
                    }
                    string returnTypeString = GetCleanReturnType(returnType);

                    MethodDetails methodDetails = new()
                    {
                        Name = name,
                        NamespaceName = classNamespace,
                        SMAPIImport = smapiImport,
                        ReturnType = returnTypeString,
                        IsReturnNull = Utils.IsTypeNullable(returnType),
                        IsList = returnTypeString.StartsWith("List") || returnTypeString.EndsWith("[]") || returnType.IsArray || returnType.IsListType() || returnType.IsDataResponse(),
                        Parameter = ps,
                        ParameterNames = string.Join(", ", parameters.Select(p => p.Name)),
                        IsGet = name.StartsWith("Get"),
                        IsGetPaged = name.StartsWith("GetPaged"),
                        IsTask = smapiAttribute.IsTask,
                        JustHub = smapiAttribute.JustHub,
                        JustController = smapiAttribute.JustController,
                        Pertsist = smapiAttribute.Persist,
                        SingalRFunction = parameter,
                        TSName = name,
                        TsParameter = ps == "" ? "" : parameter,
                        TsReturnType = GetCleanTSReturnType(returnType),
                        ReturnEntityType = GetTSTypeReturnName(returnType),
                    };

                    string? returnEntity = Utils.IsTSGeneric(Utils.ExtractInnermostType(methodDetails.ReturnType));
                    if (returnEntity != null)
                    {
                        string aa = returnEntity;

                    }


                    if (recordType.Name.StartsWith(toCheck))
                    {
                    }
                    methodDetailsList.Add(methodDetails);

                }

                foreach (KeyValuePair<string, List<MethodDetails>> kvp in methodsByNamespace)
                {
                    if (writeFiles)
                    {
                        string namespaceName = kvp.Key;
                        List<MethodDetails> methods = kvp.Value;
                        List<MethodDetails> pagedMethods = methods.Where(a => a.Name.StartsWith("Get")).ToList();

                        string fileName = Path.Combine(CSharpFileNamePrefix, namespaceName, "ControllerAndHub.cs");
                        string IFileName = Path.Combine(CSharpFileNamePrefix, namespaceName, "IControllerAndHub.cs");
                        CSharpGenerator.GenerateFile(namespaceName, methods, fileName, IFileName);

                        string tsCommandFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Commands.ts");
                        string tsTypeFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Types.ts");
                        TypeScriptCommandGenerator.GenerateFile(methods, tsCommandFilePath);

                        string tsFetchFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName);
                        TypeScriptFetchGenerator.GenerateFile(namespaceName, methods, tsFetchFilePath);


                        string tsSliceFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName);
                        TypeScriptSliceGenerator.GenerateFile(pagedMethods, tsSliceFilePath);

                        string tsHookFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName);
                        TypeScriptHookGenerator.GenerateFile(methods, tsHookFilePath);
                    }

                }

                string tsSignalRFilePath = Path.Combine(SignalRFilePathPrefix, "SignalRProvider.tsx");
                SignalRGenerator.GenerateFile([.. methodsByNamespace.SelectMany(a => a.Value).OrderBy(a => a.Name)], tsSignalRFilePath);

                string tsStoreFilePath = Path.Combine(StoreFilePathPrefix, "reducers.ts");
                StoreGenerator.GenerateFile([.. methodsByNamespace.SelectMany(a => a.Value).Where(a => a.IsGet).OrderBy(a => a.Name)], tsStoreFilePath);

                DataRefreshService.GenerateFile(methodsByNamespace, DataRefreshFilePath, IDataRefreshFilePath);

                SignalRGeneratorDataRefreshAll.GenerateFile([.. methodsByNamespace.SelectMany(a => a.Value).Where(a => a.IsGet).OrderBy(a => a.Name)], DataRefreshAllFilePath);

                //string refreshFilePath = Path.Combine(SignalRFilePathPrefix, "useDataRefresh.tsx");
                //SignalRGeneratorDataRefresh.GenerateFile(methodsByNamespace.SelectMany(a => a.Value).OrderBy(a => a.ProfileName).ToList(), refreshFilePath);

                //string setFieldFilePath = Path.Combine(SignalRFilePathPrefix, "useSetField.tsx");
                //SignalRGeneratorSetField.GenerateFile(methodsByNamespace.SelectMany(a => a.Value).ToList(), setFieldFilePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scanning for [SMAPI] methods in StreamMaster.Application. {ex}");
            }
        }

        private static string GetNameSpaceParent(Type type)
        {
            string ret = type.Namespace ?? "";
            if (ret.StartsWith(AssemblyName))
            {
                ret = ret.Remove(0, AssemblyName.Length + 1);
                if (ret.Contains("."))
                {
                    ret = ret.Remove(ret.IndexOf("."));
                }
            }
            return ret;

        }

        private static string GetCleanReturnType(Type returnType)
        {

            if (returnType.Name.Contains("Nullable"))
            {
            }

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    Type resultType = returnType.GetGenericArguments()[0];
                    return Utils.IsTypeNullable(resultType) ? FormatNullableTypeName(resultType) : FormatTypeName(resultType);
                }
                else
                {
                    return "void";
                }
            }

            if (returnType.IsGenericType)
            {
                Type resultType = returnType.GetGenericArguments()[0];
                return Utils.IsTypeNullable(resultType) ? FormatNullableTypeName(resultType) : FormatTypeName(resultType);
            }

            return Utils.IsTypeNullable(returnType) ? FormatNullableTypeName(returnType) : FormatTypeName(returnType);
        }

        private static string GetCleanTSReturnType(Type returnType)
        {
            if (returnType.Name.StartsWith("EPGFilePreviewDto"))
            {
            }

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    Type resultType = returnType.GetGenericArguments()[0];
                    if (returnType.Name.StartsWith("EPGFilePreviewDto"))
                    {
                    }
                    return FormatTSTypeName(resultType);
                }
                else
                {
                    return "void";
                }
            }
            else
            {
                return FormatTSTypeName(returnType);
            }
        }

        public static string GetTSTypeReturnName(Type type)
        {
            if (type.IsGenericType)
            {

                string genericArgs = FormatTSTypeName(type.GetGenericArguments()[0]);
                return genericArgs;
            }
            else
            {
                return FormatTSTypeName(type);
            }
        }

        private static string FormatTSTypeName(Type type)
        {

            if (type.IsGenericType)
            {
                string typeName = type.GetGenericTypeDefinition().Name;
                typeName = typeName[..typeName.IndexOf('`')];
                string genericArgs = string.Join(", ", type.GetGenericArguments().Select(FormatTSTypeName));

                if (genericArgs.StartsWith("EPGFilePreviewDto"))
                {
                }
                return type.IsListType() || type.IsArray
                    ? genericArgs.EndsWith("[]") ? genericArgs :
                    $"{genericArgs}[]"
                    : genericArgs;
            }
            else
            {
                return type.Name switch
                {
                    "String" => "string",
                    "Int32" => "number",
                    "Boolean" => "boolean",
                    _ => type.Name,
                };
            }
        }
        private static string FormatTypeName(Type type)
        {
            if (type.IsGenericType)

            {
                string typeName = type.GetGenericTypeDefinition().Name;
                typeName = typeName[..typeName.IndexOf('`')];
                string genericArgs = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
                return $"{typeName}<{genericArgs}>";
            }
            else
            {
                return type.Name switch
                {
                    "String" => "string",
                    "Int32" => "int",
                    "Boolean" => "bool",
                    _ => type.Name,
                };
            }
        }
        private static string FormatNullableTypeName(Type type)
        {
            string name = FormatTypeName(type);
            return name += "?";
        }
    }

}
