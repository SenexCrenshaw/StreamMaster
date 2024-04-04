using System.Reflection;

namespace BuildClientAPI.CSharp;

internal static class CSharpUtils
{
    public static string ParamsToCSharp(Type recordType)
    {
        if (recordType.Name == "ProcessM3UFileRequest")
        {
            int aa = 1;
        }

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
                int aa = 1;
            }
            stringBuilder.Add($"{csSharptsType} {name}");
        }
        string ret = string.Join(", ", stringBuilder);

        return ret;
    }
}
