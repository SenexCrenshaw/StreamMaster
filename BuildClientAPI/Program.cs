using MediatR;

using StreamMaster.Domain.Attributes;

using System.Reflection;

namespace BuildClientAPI
{
    internal class Program
    {
        private const string AssemblyName = "StreamMaster.Application";
        private const string CSharpFileNamePrefix = @"..\..\..\..\StreamMaster.Application\";
        private const string SMAPIFileNamePrefix = @"..\..\..\..\streammasterwebui\lib\smAPI";
        private const string SignalRFilePathPrefix = @"..\..\..\..\streammasterwebui\lib\signalr";

        private static void Main(string[] args)
        {
            ScanForSMAPI();
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

                    string ps = Util.ParamsToCSharp(recordType);
                    string tsps = Util.CSharpParamToTS(recordType);

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

                    if (recordType.Name.StartsWith("GetIcons"))
                    {
                        string returntype = GetCleanReturnType(returnType);
                        string returntypeTS = GetCleanTSReturnType(returnType);
                        string Parameters = Util.ParamsToCSharp(recordType);
                        string TsParameters = Util.CSharpParamToTS(recordType);
                        //string testI = Util.CSharpPropsToTSInterface(returnType);
                        string TsReturnType = GetCleanTSReturnType(returnType);
                        string genericArgs = string.Join(", ", recordType.GetGenericArguments().Select(FormatTypeName));
                        string genericArgs2 = string.Join(", ", returnType.GetGenericArguments().Select(FormatTypeName));
                        smapiImport.AddRange(recordType.GetGenericArguments().Select(FormatTypeName));
                        smapiImport.AddRange(returnType.GetGenericArguments().Select(FormatTypeName));

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

                    MethodDetails methodDetails = new()
                    {
                        Name = name,
                        NamespaceName = classNamespace,
                        SMAPIImport = smapiImport,
                        ReturnType = GetCleanReturnType(returnType),

                        IsList = returnType.Name.StartsWith("List"),
                        Parameter = ps,
                        ParameterNames = string.Join(", ", parameters.Select(p => p.Name)),
                        IsGet = name.StartsWith("Get"),
                        IsGetPaged = name.StartsWith("GetPaged"),
                        IsTask = smapiAttribute.IsTask,
                        JustHub = smapiAttribute.JustHub,
                        JustController = smapiAttribute.JustController,

                        SingalRFunction = parameter,

                        TSName = name,
                        TsParameter = ps == "" ? "" : parameter,
                        TsReturnType = GetCleanTSReturnType(returnType),

                        ReturnEntityType = GetTSTypeReturnName(returnType),
                    };

                    string? returnEntity = Util.IsTSGeneric(Util.ExtractInnermostType(methodDetails.ReturnType));
                    if (returnEntity != null)
                    {
                        string aa = returnEntity;

                    }

                    if (recordType.Name.StartsWith("AddSMStreamToSMChannel"))
                    {
                        List<ParameterInfo> test = parameters.ToList();
                        List<string> aa = parameters.Select(p => $"{p.ParameterType.Name}").ToList();
                        List<Type> aaa = Util.GetConstructorAndParameterTypes(recordType);
                        List<Type> aaa2 = Util.GetConstructorAndParameterTypes(returnType);
                    }

                    if (recordType.Name.StartsWith("GetIcons"))
                    {

                        int aaa = 1;
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
                        //string entityName = ParameterConverter2.ExtractInnermostType(pagedMethods.First().ReturnType);

                        string fileName = Path.Combine(CSharpFileNamePrefix, namespaceName, "ControllerAndHub.cs");
                        string IFileName = Path.Combine(CSharpFileNamePrefix, namespaceName, "IControllerAndHub.cs");
                        CSharpGenerator.GenerateFile(namespaceName, methods, fileName, IFileName);

                        string tsCommandFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Commands.ts");
                        string tsTypeFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Types.ts");
                        TypeScriptCommandGenerator.GenerateFile(methods, namespaceName, tsCommandFilePath, tsTypeFilePath);

                        string tsFetchFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Fetch.ts");
                        TypeScriptFetchGenerator.GenerateFile(namespaceName, methods, tsFetchFilePath);


                        string tsSliceFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName);
                        TypeScriptSliceGenerator.GenerateFile(pagedMethods, tsSliceFilePath);

                        string tsHookFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName);
                        TypeScriptHookGenerator.GenerateFile(methods, tsHookFilePath);

                    }

                }


                string tsSignalRFilePath = Path.Combine(SignalRFilePathPrefix, "SignalRProvider.tsx");
                SignalRGenerator.GenerateFile(methodsByNamespace.SelectMany(a => a.Value).ToList(), tsSignalRFilePath);
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
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    Type resultType = returnType.GetGenericArguments()[0];
                    return FormatTypeName(resultType);
                }
                else
                {
                    return "void";
                }
            }
            else
            {
                return FormatTypeName(returnType);
            }
        }

        private static string GetCleanTSReturnType(Type returnType)
        {
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    Type resultType = returnType.GetGenericArguments()[0];
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
                return type.Name switch
                {
                    "String" => "string",
                    "Int32" => "int",
                    "Boolean" => "boolean",

                    _ => type.Name,
                };
            }
        }

        private static string FormatTSTypeName(Type type)
        {
            if (type.IsGenericType)

            {
                string typeName = type.GetGenericTypeDefinition().Name;
                typeName = typeName[..typeName.IndexOf('`')];
                string genericArgs = string.Join(", ", type.GetGenericArguments().Select(FormatTSTypeName));
                return $"{genericArgs}[]";
            }
            else
            {
                return type.Name switch
                {
                    "String" => "string",
                    "Int32" => "int",
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
                    // Add more type mappings as necessary
                    _ => type.Name,
                };
            }
        }
    }

}
