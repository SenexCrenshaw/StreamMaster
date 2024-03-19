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
            try
            {
                Assembly assembly = Assembly.Load(AssemblyName);
                Dictionary<string, List<MethodDetails>> methodsByNamespace = [];

                foreach (Type type in assembly.GetTypes())
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        SMAPIAttribute? smapiAttribute = method.GetCustomAttribute<SMAPIAttribute>();
                        if (smapiAttribute != null)
                        {
                            string classNamespace = GetNameSpaceParent(type);
                            if (!methodsByNamespace.TryGetValue(classNamespace, out List<MethodDetails> methodDetailsList))
                            {
                                methodDetailsList = [];
                                methodsByNamespace[classNamespace] = methodDetailsList;
                            }

                            MethodDetails methodDetails = new()
                            {
                                Name = method.Name,
                                ReturnType = GetCleanReturnType(method),
                                Parameters = string.Join(", ", method.GetParameters().Select(p => $"{TypeStandardizer.GetStandardType(p.ParameterType.Name)} {p.Name}")),
                                ParameterNames = string.Join(", ", method.GetParameters().Select(p => p.Name)),
                                IncludeInHub = true
                            };

                            methodDetailsList.Add(methodDetails);
                        }
                    }
                }

                foreach (KeyValuePair<string, List<MethodDetails>> kvp in methodsByNamespace)
                {
                    string namespaceName = kvp.Key;
                    List<MethodDetails> methods = kvp.Value;
                    string fileName = Path.Combine(CSharpFileNamePrefix, namespaceName, "Commands.cs");
                    CSharpGenerator.GenerateFile(namespaceName, methods, fileName);

                    string tsCommandFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Commands.ts");
                    TypeScriptCommandGenerator.GenerateFile(namespaceName, methods, tsCommandFilePath);

                    string tsFetchFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Fetch.ts");
                    TypeScriptFetchGenerator.GenerateFile(namespaceName, methods, tsFetchFilePath);

                    List<MethodDetails> pagedMethods = methods.Where(a => a.Name.StartsWith("GetPaged")).ToList();
                    if (pagedMethods.Count > 0)
                    {
                        string tsSliceFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"{namespaceName}Slice.ts");
                        TypeScriptSliceGenerator.GenerateFile(namespaceName, pagedMethods, tsSliceFilePath);

                        string entityName = ParameterConverter.ExtractInnermostType(pagedMethods.First().ReturnType);
                        string tsHookFilePath = Path.Combine(SMAPIFileNamePrefix, namespaceName, $"use{namespaceName}.ts");
                        TypeScriptHookGenerator.GenerateFile(namespaceName, entityName, methods, tsHookFilePath);

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

        private static string GetCleanReturnType(MethodInfo method)
        {
            Type returnType = method.ReturnType;

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
