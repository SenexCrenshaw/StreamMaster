using System.Text;

public static class SignalRGeneratorDataRefresh
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

        content.AppendLine($"export const useDataRefresh = (");
        //content.AppendLine($"{psList}");
        content.AppendLine($") =>");
        content.AppendLine($"  useCallback(");
        content.AppendLine($"    (");
        content.AppendLine($"      entity:");

        ps = methods.Select(a => $"        | '{a.Name}'");
        psList = string.Join("\r\n", ps);
        content.AppendLine($"{psList}");

        content.AppendLine("    ): void => {");
        content.AppendLine("      const entityMap: { [key: string]: { SetIsForced: (isForced: boolean) => void } } = {");

        ps = methods.Select(a => $"        {a.Name}: use{a.Name}()");
        psList = string.Join(",\r\n", ps);
        content.AppendLine($"{psList}");
        content.AppendLine("      };");

        content.AppendLine();
        content.AppendLine("      const action = entityMap[entity];");
        content.AppendLine("      if (action) {");
        content.AppendLine("        action.SetIsForced(true);");
        content.AppendLine("      } else {");
        content.AppendLine("        console.error(`Unknown entity: ${entity}`);");
        content.AppendLine("      }");
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
        foreach (MethodDetails method in methods)
        {
            content.AppendLine($"import use{method.Name} from '@lib/smAPI/{method.NamespaceName}/use{method.Name}';");
        }

        content.AppendLine();
        return content.ToString();
    }

}
