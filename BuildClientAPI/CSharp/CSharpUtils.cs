using System.Reflection;

namespace BuildClientAPI.CSharp;

internal static class CSharpUtils
{
    public static bool IsDataResponse(this Type typeToCheck)
    {
        try
        {
            if (typeToCheck == null)
            {
                throw new ArgumentNullException(nameof(typeToCheck), "The type to check cannot be null.");
            }

            // Check if the type is a generic type and if the generic type definition is DataResponse
            if (typeToCheck.IsGenericType &&
                typeToCheck.GetGenericTypeDefinition().FullName == "StreamMaster.Domain.API.DataResponse`1")
            {
                // Extract the generic arguments of the DataResponse type
                Type? genericArgument = typeToCheck.GetGenericArguments().FirstOrDefault();

                // Now we check if the generic argument is the expected type, in this case, List<LogoFileDto>
                if (genericArgument?.IsGenericType == true &&
                    genericArgument.GetGenericTypeDefinition() == typeof(List<>) &&
                    genericArgument.GetGenericArguments().FirstOrDefault()?.FullName == "StreamMaster.Domain.Dto.LogoFileDto")
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while trying to check if {TypeToCheck} is part of DataResponse.: {ex}", typeToCheck, ex);
            // Depending on requirements, you may want to handle this differently
            // For example, re-throwing the exception, returning false, or handling specific exceptions differently
        }

        return false;
    }
    public static string ParamsToCSharp(Type recordType)
    {

        List<string> stringBuilder = [];
        ConstructorInfo[] constructors = recordType.GetConstructors();

        ParameterInfo[] parameters = constructors[0].GetParameters();
        foreach (ParameterInfo p in parameters)
        {


            string? name = p.Name;
            Type pType = p.ParameterType;
            string csSharptsTypeFullName = Utils.GetTypeFullNameForParameter(p.ParameterType);
            string csSharptsType = Utils.CleanupTypeName(csSharptsTypeFullName);
            if (Utils.IsParameterNullable(p))
            {
            }
            stringBuilder.Add($"{csSharptsType} {name}");
        }
        string ret = string.Join(", ", stringBuilder);

        return ret;
    }
}
