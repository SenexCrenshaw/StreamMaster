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
        private const string SMAPISliceNamePrefix = @"..\..\..\..\streammasterwebui\lib\redux\slices";

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
                    string classNamespace = GetNameSpaceParent(recordType);
                    if (!methodsByNamespace.TryGetValue(classNamespace, out List<MethodDetails> methodDetailsList))
                    {
                        methodDetailsList = [];
                        methodsByNamespace[classNamespace] = methodDetailsList;

                    }


                    string ps = Util.ParamsToCSharp(recordType);
                    string tsps = Util.CSharpParamToTS(recordType); ;

                    ConstructorInfo[] constructors = recordType.GetConstructors();
                    ParameterInfo[] parameters = constructors[0].GetParameters();

                    Type? iRequestInterface = recordType.GetInterfaces()
                                                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

                    if (iRequestInterface == null)
                    {
                        continue;
                    }

                    Type returnType = iRequestInterface.GenericTypeArguments[0];


                    if (recordType.Name == "GetSettings")
                    {
                        string returntype = GetCleanReturnType(recordType);
                        string Parameters = Util.ParamsToCSharp(recordType);
                        string TsParameters = Util.CSharpParamToTS(recordType);
                        string testI = Util.CSharpPropsToTSInterface(returnType);
                    }


                    string name = recordType.Name;
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
                        ReturnType = GetCleanReturnType(returnType),
                        Parameters = ps,
                        ParameterNames = string.Join(", ", parameters.Select(p => p.Name)),
                        TsParameters = tsps,
                        TsParameterTypes = string.Join(", ", parameters.Select(p => Util.MapCSharpTypeToTypeScript(Util.GetTypeFullNameForParameter(p.ParameterType)))),
                        IsGetPaged = name.StartsWith("GetPaged"),
                        IsTask = smapiAttribute.IsTask,
                        JustHub = smapiAttribute.JustHub,
                        JustController = smapiAttribute.JustController,
                        TsReturnInterface = ""//Util.CSharpPropsToTSInterface(returnType)
                    };

                    string? returnEntity = Util.IsTSGeneric(Util.ExtractInnermostType(methodDetails.ReturnType));
                    if (returnEntity != null)
                    {
                        string aa = returnEntity;

                    }

                    if (recordType.Name == "AddSMStreamToSMChannel")
                    {
                        List<ParameterInfo> test = parameters.ToList();
                        List<string> aa = parameters.Select(p => $"{p.ParameterType.Name}").ToList();
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

                        //if (pagedMethods.Count > 0)
                        //{
                        string tsSliceFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Slice.ts");
                        TypeScriptSliceGenerator.GenerateFile(namespaceName, pagedMethods, tsSliceFilePath);


                        string tsHookFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"use{namespaceName}.ts");
                        TypeScriptHookGenerator.GenerateFile(namespaceName, methods, tsHookFilePath);

                        //}
                    }

                }
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
