using System.Text;

using BuildClientAPI.Models;
namespace BuildClientAPI.TS;
public static class StoreGenerator
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();

        content.Append(AddImports(methods));
        content.Append(GenerateConfigs());

        content.Append(GenerateReducer(methods));

        DirectoryInfo? directoryInfo = Directory.GetParent(filePath) ?? throw new ApplicationException($"Could not get directory information from file path {filePath}");
        if (!Directory.Exists(directoryInfo.FullName))
        {
            Directory.CreateDirectory(directoryInfo.FullName);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static readonly List<string> Persists =
    [
        "selectedPostalCode",
        "selectAll",
        "selectedCountry",
        "selectedItems",
        "selectedStreamGroup",
        "showHidden",
        "showSelected",
        "showSelections",
        "sortInfo",
        "selectedSMStreams",
        "selectedSMChannel",
        "queryFilter",
        "selectedSMStream",
        "queryAdditionalFilters",
          "filters",
        "isTrue",
        "stringValue"
    ];

    private static readonly List<string> AdditionalReducers =
    [
        "messages",
        "updateSettingRequest",
        "currentSettingRequest",
        "loading"
    ];

    private static string GenerateConfigs()
    {
        StringBuilder content = new();
        foreach (string? persist in Persists.Order())
        {
            content.AppendLine($"const {persist}Config = {{");
            content.AppendLine($"  key: '{persist}',");
            content.AppendLine("  storage");
            content.AppendLine("};");
        }
        return content.ToString();
    }

    private static string GenerateReducer(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        Dictionary<string, string> reducers = [];// new()
        foreach (MethodDetails method in methods)
        {
            reducers[method.Name] = $"  {method.Name}: {method.Name}Reducer";
        }

        foreach (string additional in AdditionalReducers)
        {
            reducers[additional] = $"  {additional}: {additional}";
        }

        foreach (string persist in Persists)
        {
            reducers[persist] = $"  {persist}: persistReducer({persist}Config, {persist})";
        }

        content.AppendLine("export const rootReducer = combineReducers({");

        foreach (string? key in reducers.Keys.Order())
        {
            content.AppendLine(reducers[key] + ",");
        }

        content.AppendLine("});");
        content.AppendLine("");

        return content.ToString();
    }

    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        Dictionary<string, string> imports = [];

        content.AppendLine("import { combineReducers } from 'redux';");

        foreach (MethodDetails method in methods.Where(a => a.IsGet))
        {
            imports[method.Name] = $"import {method.Name}Reducer from '@lib/smAPI/{method.NamespaceName}/{method.Name}Slice';";
        }

        foreach (string persist in Persists)
        {
            imports[persist] = $"import {persist} from '@lib/redux/hooks/{persist}';";
        }

        foreach (string additional in AdditionalReducers)
        {
            imports[additional] = $"import {additional} from '@lib/redux/hooks/{additional}';";
        }

        foreach (string? key in imports.Keys.Order())
        {
            content.AppendLine(imports[key]);
        }

        content.AppendLine("import { persistReducer } from 'redux-persist';");
        content.AppendLine("import storage from 'redux-persist/lib/storage';");

        content.AppendLine();
        return content.ToString();
    }
}
