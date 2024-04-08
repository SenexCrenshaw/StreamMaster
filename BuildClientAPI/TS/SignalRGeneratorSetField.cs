using System.Text;

public static class SignalRGeneratorSetField
{
    public static void GenerateFile(List<MethodDetails> methods, string filePath)
    {
        StringBuilder content = new();
        methods = methods.Where(a => a.IsGet).ToList();

        content.Append(AddImports(methods));
        content.Append(GenerateHook(methods));

        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, content.ToString());
    }

    private static string GenerateHook(List<MethodDetails> methods)
    {

        StringBuilder content = new();

        IEnumerable<string> ps = methods.Select(a => $"  {a.Name.ToCamelCase()}: any");
        string psList = string.Join(",\r\n", ps);

        content.AppendLine($"export const useSetField = () =>");
        //content.AppendLine($"{psList}");        
        content.AppendLine($"  useCallback(");
        content.AppendLine($"    (fieldDatas: FieldData[]): void => {{");
        content.AppendLine($"      fieldDatas.forEach((fieldData) => {{");
        content.AppendLine($"        const entityMap: {{ [key: string]: {{ SetField: (fieldData: FieldData) => void }} }} = {{");

        ps = methods.Select(a => $"        {a.Name}: use{a.Name}()");
        psList = string.Join(",\r\n", ps);
        content.AppendLine($"{psList},");
        content.AppendLine("        };");

        content.AppendLine();
        content.AppendLine("        const action = entityMap[fieldData.Entity];");
        content.AppendLine("        if (action) {");
        content.AppendLine("          action.SetField(fieldData);");
        content.AppendLine("        } else {");
        content.AppendLine("          console.error(`Unknown entity: ${fieldData.Entity}`);");
        content.AppendLine("        }");
        content.AppendLine("      });");
        content.AppendLine("    },");
        content.AppendLine("    [");

        ps = methods.Select(a => $"      use{a.Name}");
        psList = string.Join(",\r\n", ps);
        content.AppendLine($"{psList}");
        content.AppendLine($"    ]");
        content.AppendLine($"  );");

        return content.ToString();
    }


    private static string AddImports(List<MethodDetails> methods)
    {
        StringBuilder content = new();
        content.AppendLine("import { useCallback } from 'react';");
        content.AppendLine("import { FieldData } from '@lib/smAPI/smapiTypes';");
        foreach (MethodDetails method in methods)
        {
            content.AppendLine($"import use{method.Name} from '@lib/smAPI/{method.NamespaceName}/use{method.Name}';");
        }

        content.AppendLine();
        return content.ToString();
    }

}
