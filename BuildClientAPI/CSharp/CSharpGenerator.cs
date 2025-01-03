using System.Text;

using BuildClientAPI.Models;

using StreamMaster.Domain.Extensions;
namespace BuildClientAPI.CSharp;

public static class CSharpGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath, string IFilePath)
    {
        StringBuilder IcontrollerContent = new();
        StringBuilder controllerContent = new();

        StringBuilder IhubContent = new();
        StringBuilder hubContent = new();

        bool needsQueryInclude = methods.Any(a => a.IsGet);
        bool needsCommandInclude = methods.Any(a => !a.IsGet);
        bool needsLogger = false;

        foreach (MethodDetails method in methods)
        {
            // Determine HTTP method attribute (simplified example)
            string httpAttribute = "HttpPatch";
            if (method.Name.StartsWith("Get"))
            {
                httpAttribute = "HttpGet";
            }
            else if (method.Name.StartsWith("Delete"))
            {
                httpAttribute = "HttpDelete";
            }
            else if (method.Name.StartsWith("Remove"))
            {
                httpAttribute = "HttpDelete";
            }
            else if (method.Name.StartsWith("Create"))
            {
                httpAttribute = "HttpPost";
            }

            const string route = "[Route(\"[action]\")]";
            string httpMethodLine = $"[{httpAttribute}]";
            string parameterLine = string.IsNullOrEmpty(method.Parameter) ? "" : $"{method.Name}Request request";
            string toSend = string.IsNullOrEmpty(method.Parameter) ? $"new {method.SingalRFunction}()" : "request";
            string toReturn = method.ReturnType;
            string nullReturn = "";

            if ((method.IsGet || method.IsGetPaged) && method.IsReturnNull)
            {
                if (!method.IsReturnNull)
                {
                }
                nullReturn = toReturn.Contains("List<") ? "?? []" : "?? new()";
                toReturn = method.ReturnType[..^1];
                if (!method.IsList)
                {
                    Console.WriteLine(toReturn);
                    if (toReturn.EqualsIgnoreCase("string"))
                    {
                        nullReturn = "?? string.Empty";
                    }
                    if (toReturn.EqualsIgnoreCase("bool"))
                    {
                        nullReturn = "?? false";
                    }

                    if (toReturn.EqualsIgnoreCase("int"))
                    {
                        nullReturn = "?? 0";
                    }
                }
            }
            if (method.Name.ContainsIgnoreCase("GetIsSystemReady"))
            {
            }

            if (!method.JustHub)
            {
                controllerContent.AppendLine($"        {httpMethodLine}");
                controllerContent.AppendLine($"        {route}");

                if (method.IsGetPaged)
                {
                    const string fromQ = "[FromQuery] ";
                    controllerContent.AppendLine($"        public async Task<ActionResult<PagedResponse<{toReturn}>>> {method.Name}({fromQ}{method.Parameter})");
                    controllerContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        controllerContent.AppendLine($"            var ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    }
                    else
                    {
                        controllerContent.AppendLine($"            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new {method.SingalRFunction}({method.ParameterNames}))).ConfigureAwait(false);");
                    }
                    controllerContent.AppendLine($"            return ret{nullReturn};");
                    IcontrollerContent.AppendLine($"        Task<ActionResult<PagedResponse<{toReturn}>>> {method.Name}({method.Parameter});");
                }
                else if (method.IsGetCached)
                {
                    const string fromQ = "[FromQuery] ";
                    needsLogger = true;
                    controllerContent.AppendLine($"        public async Task<ActionResult<{toReturn}>> {method.Name}({fromQ}{method.Name}Request request)");
                    controllerContent.AppendLine("        {");
                    controllerContent.AppendLine("            try");
                    controllerContent.AppendLine("            {");
                    if (method.NoDebug)
                    {
                        controllerContent.AppendLine("            var ret = await Sender.Send(request).ConfigureAwait(false);");
                    }
                    else
                    {
                        controllerContent.AppendLine("            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);");
                    }
                    controllerContent.AppendLine($"             return ret.IsError ? Problem(detail: \"An unexpected error occurred retrieving {method.Name}.\", statusCode: 500) : Ok(ret.Data{nullReturn});");
                    controllerContent.AppendLine("            }");
                    controllerContent.AppendLine("            catch (Exception ex)");
                    controllerContent.AppendLine("            {");
                    controllerContent.AppendLine($"                _logger.LogError(ex, \"An unexpected error occurred while processing the request to get {method.Name}.\");");
                    controllerContent.AppendLine("                return Problem(detail: \"An unexpected error occurred. Please try again later.\", statusCode: 500);");
                    controllerContent.AppendLine("            }");

                    IcontrollerContent.AppendLine($"        Task<ActionResult<{toReturn}>> {method.Name}({method.Name}Request request);");
                }
                else if (method.IsGet)
                {
                    needsLogger = true;
                    controllerContent.AppendLine($"        public async Task<ActionResult<{toReturn}>> {method.Name}({method.Parameter})");
                    controllerContent.AppendLine("        {");
                    controllerContent.AppendLine("            try");
                    controllerContent.AppendLine("            {");
                    if (method.NoDebug)
                    {
                        controllerContent.AppendLine($"            var ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    }
                    else
                    {
                        controllerContent.AppendLine($"            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new {method.SingalRFunction}({method.ParameterNames}))).ConfigureAwait(false);");
                    }
                    controllerContent.AppendLine($"             return ret.IsError ? Problem(detail: \"An unexpected error occurred retrieving {method.Name}.\", statusCode: 500) : Ok(ret.Data{nullReturn});");
                    controllerContent.AppendLine("            }");
                    controllerContent.AppendLine("            catch (Exception ex)");
                    controllerContent.AppendLine("            {");
                    controllerContent.AppendLine($"                _logger.LogError(ex, \"An unexpected error occurred while processing the request to get {method.Name}.\");");
                    controllerContent.AppendLine("                return Problem(detail: \"An unexpected error occurred. Please try again later.\", statusCode: 500);");
                    controllerContent.AppendLine("            }");

                    IcontrollerContent.AppendLine($"        Task<ActionResult<{toReturn}>> {method.Name}({method.Parameter});");
                }
                else if (method.IsTask)
                {
                    controllerContent.AppendLine($"        public async Task<{toReturn}> {method.Name}({method.TsParameter} request)");
                    controllerContent.AppendLine("        {");
                    controllerContent.AppendLine($"            var ret = await taskQueue{method.Name}(request).ConfigureAwait(false);");
                    IcontrollerContent.AppendLine($"        Task<{toReturn}> {method.Name}({method.Name}Request request);");
                }
                else
                {
                    controllerContent.AppendLine($"        public async Task<ActionResult<{toReturn}>> {method.Name}({parameterLine})");
                    controllerContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        controllerContent.AppendLine($"            var ret = await Sender.Send({toSend}).ConfigureAwait(false);");
                    }
                    else
                    {
                        controllerContent.AppendLine($"            var ret = await APIStatsLogger.DebugAPI(Sender.Send({toSend})).ConfigureAwait(false);");
                    }
                    if (method.IsReturnNull)
                    {
                        controllerContent.AppendLine("            return ret == null ? NotFound(ret) : Ok(ret);");
                    }
                    else
                    {
                        controllerContent.AppendLine("            return Ok(ret);");
                    }
                    IcontrollerContent.AppendLine($"        Task<ActionResult<{toReturn}>> {method.Name}({parameterLine});");
                }

                controllerContent.AppendLine("        }");
                //controllerContent.AppendLine();
            }

            // Hub method signature (if applicable)
            if (!method.JustController)
            {
                if (method.ReturnType.Equals("APIResponse?") || (method.IsGet && method.ReturnType.EndsWith(value: '?')))
                {
                    method.ReturnType = method.ReturnType[..^1];
                }

                if (method.IsGetPaged)
                {
                    hubContent.AppendLine($"        public async Task<PagedResponse<{toReturn}>> {method.Name}({method.Parameter})");
                    hubContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        hubContent.AppendLine($"            var ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    }
                    else
                    {
                        hubContent.AppendLine($"            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new {method.SingalRFunction}({method.ParameterNames}))).ConfigureAwait(false);");
                    }
                    hubContent.AppendLine($"            return ret{nullReturn};");
                    IhubContent.AppendLine($"        Task<PagedResponse<{toReturn}>> {method.Name}({method.Parameter});");
                }
                else if (method.IsGetCached)
                {
                    hubContent.AppendLine($"        public async Task<{toReturn}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        hubContent.AppendLine("             var ret = await Sender.Send(request).ConfigureAwait(false);");
                    }
                    else
                    {
                        hubContent.AppendLine("             var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);");
                    }
                    hubContent.AppendLine($"            return ret.Data{nullReturn};");
                    IhubContent.AppendLine($"        Task<{toReturn}> {method.Name}({method.Name}Request request);");
                }
                else if (method.IsGet)
                {
                    hubContent.AppendLine($"        public async Task<{toReturn}> {method.Name}({method.Parameter})");
                    hubContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        hubContent.AppendLine($"             var ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    }
                    else
                    {
                        hubContent.AppendLine($"             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new {method.SingalRFunction}({method.ParameterNames}))).ConfigureAwait(false);");
                    }
                    hubContent.AppendLine($"            return ret.Data{nullReturn};");
                    IhubContent.AppendLine($"        Task<{toReturn}> {method.Name}({method.Parameter});");
                }
                else if (method.IsTask)
                {
                    hubContent.AppendLine($"        public async Task<{toReturn}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine("        {");
                    hubContent.AppendLine($"            await taskQueue.{method.Name}(request).ConfigureAwait(false);");
                    hubContent.AppendLine("            return APIResponse.Ok;");
                    IhubContent.AppendLine($"        Task<{toReturn}> {method.Name}({method.Name}Request request);");
                }
                else
                {
                    hubContent.AppendLine($"        public async Task<{toReturn}> {method.Name}({parameterLine})");
                    hubContent.AppendLine("        {");
                    if (method.NoDebug)
                    {
                        hubContent.AppendLine($"            var ret = await Sender.Send({toSend}).ConfigureAwait(false);");
                    }
                    else
                    {
                        hubContent.AppendLine($"            var ret = await APIStatsLogger.DebugAPI(Sender.Send({toSend})).ConfigureAwait(false);");
                    }
                    hubContent.AppendLine($"            return ret{nullReturn};");
                    IhubContent.AppendLine($"        Task<{toReturn}> {method.Name}({parameterLine});");
                }

                hubContent.AppendLine("        }");
                //hubContent.AppendLine();
            }
        }

        WriteControllerAndHub(filePath, namespaceName, controllerContent, hubContent, needsQueryInclude, needsCommandInclude, needsLogger);
        WriteIControllerAndHub(IFilePath, namespaceName, IcontrollerContent, IhubContent, needsQueryInclude, needsCommandInclude);
    }

    private static void WriteIControllerAndHub(string IFilePath, string namespaceName, StringBuilder IControllerContent, StringBuilder IHubContent, bool NeedsQueryInclude, bool NeedsCommandInclude)
    {
        string fileContent = "using Microsoft.AspNetCore.Mvc;\n";
        fileContent += "using Microsoft.AspNetCore.Authorization;\n";
        if (NeedsCommandInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Commands;\n";
        }

        if (NeedsQueryInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Queries;\n";
        }

        fileContent += $@"
namespace StreamMaster.Application.{namespaceName}
{{
    public interface I{namespaceName}Controller
    {{
{IControllerContent}    }}
}}

namespace StreamMaster.Application.Hubs
{{
    public interface I{namespaceName}Hub
    {{
{IHubContent}    }}
}}
";
        DirectoryInfo? parentDirectory = Directory.GetParent(IFilePath);
        if (parentDirectory is not null && !Directory.Exists(parentDirectory.FullName))
        {
            Directory.CreateDirectory(parentDirectory.FullName);
        }
        File.WriteAllText(IFilePath, fileContent);
    }

    private static void WriteControllerAndHub(string filePath, string namespaceName, StringBuilder controllerContent, StringBuilder hubContent, bool NeedsQueryInclude, bool NeedsCommandInclude, bool needsLogger)
    {
        string fileContent = "using Microsoft.AspNetCore.Mvc;\n";
        fileContent += "using Microsoft.AspNetCore.Authorization;\n";
        if (NeedsCommandInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Commands;\n";
        }

        if (NeedsQueryInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Queries;\n";
        }

        string ns = namespaceName + ".Controllers";

        string logger = needsLogger ? "ILogger<" + namespaceName + "Controller> _logger" : "";
        fileContent += $@"
namespace StreamMaster.Application.{ns}
{{
    [Authorize]
    public partial class {namespaceName}Controller({logger}) : ApiControllerBase, I{namespaceName}Controller
    {{
{controllerContent}    }}
}}

namespace StreamMaster.Application.Hubs
{{
    public partial class StreamMasterHub : I{namespaceName}Hub
    {{
{hubContent}    }}
}}
";
        DirectoryInfo? parentDirectory = Directory.GetParent(filePath);
        if (parentDirectory is not null && !Directory.Exists(parentDirectory.Name))
        {
            if (!Directory.Exists(parentDirectory.FullName))
            {
                Directory.CreateDirectory(parentDirectory.FullName);
            }
        }

        File.WriteAllText(filePath, fileContent);
    }
}
