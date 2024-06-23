using System.Text;

public static class SignalRGeneratorDataRefreshAll
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();

        content.Append(AddImports(methods));
        content.Append(DataRefreshAll(methods));

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string DataRefreshAll(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        List<string> deps = [];
        content.AppendLine("export const DataRefreshAll = () => {");

        foreach (MethodDetails? method in methods.Where(a => a.IsGet & a.ParameterNames == ""))
        {
            if (method.Name.Contains("GetEPGFilePreviewById"))
            {
                var a = 1;
            }
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
