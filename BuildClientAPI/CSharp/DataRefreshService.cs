using System.Text;

public static class DataRefreshService
{
    public static void GenerateFile(Dictionary<string, List<MethodDetails>> methodsByNamespace, string filePath, string IfilePath)
    {
        StringBuilder content = new();

        content.Append(AddImports());

        content.Append(GenerateMethods(methodsByNamespace));

        content.AppendLine("}");

        var interfaceContent = GenerateInterface(methodsByNamespace);
        File.WriteAllText(filePath, content.ToString());
        File.WriteAllText(IfilePath, interfaceContent);
    }

    private static string GenerateInterface(Dictionary<string, List<MethodDetails>> methodsByNamespace)
    {
        StringBuilder content = new();

        content.AppendLine("namespace StreamMaster.Domain.Services");
        content.AppendLine("{");
        content.AppendLine("    public interface IDataRefreshService: IDataRefreshServicePartial");
        content.AppendLine("    {");
        content.AppendLine("        Task SetField(List<FieldData> fieldData);");
        content.AppendLine("        Task ClearByTag(string Entity, string Tag);");
        content.AppendLine("        Task RefreshAll();");

        foreach (var namespaceName in methodsByNamespace.Keys)
        {
            var gets = methodsByNamespace[namespaceName].Any(a => a.IsGet);
            if (!gets)
            {
                continue;
            }

            content.AppendLine($"        Task Refresh{namespaceName}(bool alwaysRun = false);");

        }

        content.AppendLine("    }");
        content.AppendLine("}");

        return content.ToString();

    }

    private static string GenerateMethods(Dictionary<string, List<MethodDetails>> methodsByNamespace)
    {
        StringBuilder content = new();
        content.AppendLine();
        content.AppendLine($"    public async Task RefreshAll()");
        content.AppendLine("    {");
        content.AppendLine();

        foreach (var namespaceName in methodsByNamespace.Keys.Order())
        {
            var gets = methodsByNamespace[namespaceName].Where(a => a.IsGet).OrderBy(a => a.Name).ToList();
            if (gets.Count == 0)
            {
                continue;
            }

            content.AppendLine($"        await Refresh{namespaceName}(true);");
        }
        content.AppendLine("    }");

        foreach (var namespaceName in methodsByNamespace.Keys.Order())
        {
            var gets = methodsByNamespace[namespaceName].Where(a => a.IsGet).OrderBy(a => a.Name).ToList();
            if (gets.Count == 0)
            {
                continue;
            }

            content.AppendLine("");
            content.AppendLine($"    public async Task Refresh{namespaceName}(bool alwaysRun = false)");
            content.AppendLine("    {");
            content.AppendLine();
            content.AppendLine("        if (!alwaysRun && !BuildInfo.SetIsSystemReady)");
            content.AppendLine("        {");
            content.AppendLine("            return;");
            content.AppendLine("        }");
            content.AppendLine();
            foreach (var get in gets)
            {
                content.AppendLine($"        await hub.Clients.All.DataRefresh(\"{get.Name}\");");
            }
            content.AppendLine("    }");
        }

        return content.ToString();
    }

    private static string AddImports()
    {
        StringBuilder content = new();


        content.AppendLine("using Microsoft.AspNetCore.SignalR;");
        content.AppendLine("");
        content.AppendLine("using StreamMaster.Application.Common.Interfaces;");
        content.AppendLine("using StreamMaster.Application.Hubs;");
        content.AppendLine("using StreamMaster.Domain.Configuration;");
        content.AppendLine("");
        content.AppendLine("namespace StreamMaster.Infrastructure.Services;");
        content.AppendLine("");

        content.AppendLine("public partial class DataRefreshService(IHubContext<StreamMasterHub, IStreamMasterHub> hub) : IDataRefreshService, IDataRefreshServicePartial");

        content.AppendLine("{");

        return content.ToString();
    }

}
