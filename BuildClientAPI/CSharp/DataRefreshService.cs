using System.Text;

using BuildClientAPI.Models;
namespace BuildClientAPI.CSharp;
public static class DataRefreshService
{
    public static void GenerateFile(Dictionary<string, List<MethodDetails>> methodsByNamespace, string filePath, string IfilePath)
    {
        StringBuilder content = new();

        content.Append(AddImports());

        content.Append(GenerateMethods(methodsByNamespace));

        content.AppendLine("}");

        string interfaceContent = GenerateInterface(methodsByNamespace);
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
        content.AppendLine("        Task Refresh(string command);");

        foreach (string namespaceName in methodsByNamespace.Keys)
        {
            bool gets = methodsByNamespace[namespaceName].Any(a => a.IsGet);
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
        //content.AppendLine();
        content.AppendLine("    public async Task RefreshAll()");
        content.AppendLine("    {");
        //content.AppendLine();

        foreach (string? namespaceName in methodsByNamespace.Keys.Order())
        {
            List<MethodDetails> gets = [.. methodsByNamespace[namespaceName].Where(a => a.IsGet).OrderBy(a => a.Name)];
            if (gets.Count == 0)
            {
                continue;
            }

            content.AppendLine($"        await Refresh{namespaceName}(true);");
        }
        content.AppendLine("    }");

        foreach (string? namespaceName in methodsByNamespace.Keys.Order())
        {
            List<MethodDetails> gets = [.. methodsByNamespace[namespaceName].Where(a => a.IsGet).OrderBy(a => a.Name)];
            if (gets.Count == 0)
            {
                continue;
            }

            content.AppendLine();
            content.AppendLine($"    public async Task Refresh{namespaceName}(bool alwaysRun = false)");
            content.AppendLine("    {");
            //content.AppendLine();
            content.AppendLine("        if (!alwaysRun && !BuildInfo.IsSystemReady)");
            content.AppendLine("        {");
            content.AppendLine("            return;");
            content.AppendLine("        }");
            content.AppendLine();
            List<MethodDetails> gs = gets.Where(a => a.ParameterNames != "").ToList();

            //foreach (MethodDetails? get in gets.Where(a => a.ParameterNames == "" || a.IsGetPaged))
            foreach (MethodDetails? get in gets.Where(a => a.IsGet && (a.IsGetPaged || a.ParameterNames?.Length == 0 || gets.Count == 1)))
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
        content.AppendLine("using StreamMaster.Application.Interfaces;");
        content.AppendLine("using StreamMaster.Application.Hubs;");
        content.AppendLine("using StreamMaster.Application.Services;");
        content.AppendLine("using StreamMaster.Domain.Configuration;");
        content.AppendLine("");
        content.AppendLine("namespace StreamMaster.Infrastructure.Services;");
        content.AppendLine("");

        content.AppendLine("public partial class DataRefreshService(IHubContext<StreamMasterHub, IStreamMasterHub> hub) : IDataRefreshService, IDataRefreshServicePartial");

        content.AppendLine("{");

        return content.ToString();
    }
}
