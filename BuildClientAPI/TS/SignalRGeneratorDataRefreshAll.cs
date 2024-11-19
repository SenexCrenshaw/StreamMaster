using System.Text;
namespace BuildClientAPI.TS;
public static class SignalRGeneratorDataRefreshAll
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();

        content.Append(AddImports(methods));
        content.Append(DataRefreshAll(methods));

        DirectoryInfo? directoryInfo = Directory.GetParent(filePath) ?? throw new ApplicationException($"Could not get directory information from file path {filePath}");
        if (!Directory.Exists(directoryInfo.FullName))
        {
            Directory.CreateDirectory(directoryInfo.FullName);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string DataRefreshAll(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("export const DataRefreshAll = () => {");

        foreach (MethodDetails? method in methods.Where(a => a.IsGet && a.ParameterNames == ""))
        {
            content.AppendLine($"  store.dispatch({method.Name}SetIsForced({{ force: true }}));");
        }

        content.AppendLine("};");
        return content.ToString();
    }

    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        content.AppendLine("import store from '@lib/redux/store';");

        foreach (MethodDetails method in methods.Where(a => a.IsGet & a.ParameterNames == ""))
        {
            content.AppendLine($"import {{ setIsForced as {method.Name}SetIsForced }} from '@lib/smAPI/{method.NamespaceName}/{method.Name}Slice';");
        }

        content.AppendLine();
        return content.ToString();
    }
}
