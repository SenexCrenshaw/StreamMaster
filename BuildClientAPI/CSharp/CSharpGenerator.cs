using System.Text;

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

            string route = $"[Route(\"[action]\")]";
            string httpMethodLine = $"[{httpAttribute}]";
            string parameterLine = string.IsNullOrEmpty(method.Parameter) ? "" : $"{method.Name}Request request";
            string toSend = string.IsNullOrEmpty(method.Parameter) ? $"new {method.SingalRFunction}()" : "request";

            if (method.Name == "SendSMTaskRequest")
            {
            }

            if (!method.JustHub)
            {
                controllerContent.AppendLine($"        {httpMethodLine}");
                controllerContent.AppendLine($"        {route}");

                if (method.Name == "GetIcons")
                {
                }

                if (method.ReturnType.Equals("APIResponse?") || (method.IsGet && method.ReturnType.EndsWith("?")))
                {
                    method.ReturnType = method.ReturnType[..^1];
                }

                if (method.IsGetPaged)
                {
                    string fromQ = "[FromQuery] ";
                    controllerContent.AppendLine($"        public async Task<ActionResult<PagedResponse<{method.ReturnType}>>> {method.Name}({fromQ}{method.Parameter})");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            PagedResponse<{method.ReturnType}> ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    controllerContent.AppendLine($"            return ret;");
                    IcontrollerContent.AppendLine($"        Task<ActionResult<PagedResponse<{method.ReturnType}>>> {method.Name}({method.Parameter});");
                }
                else if (method.IsGetCached)
                {
                    string fromQ = "[FromQuery] ";
                    //if (method.ProfileName == "GetIcons")
                    //{
                    //    int aa = 1;
                    //     fromQ = "[FromQuery] ";
                    //}
                    //else
                    //{

                    //}

                    controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({fromQ}{method.Name}Request request)");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            try");
                    controllerContent.AppendLine($"            {{");
                    controllerContent.AppendLine($"            DataResponse<{method.ReturnType}> ret = await Sender.Send(request).ConfigureAwait(false);");
                    controllerContent.AppendLine($"             return ret.IsError ? Problem(detail: \"An unexpected error occurred retrieving {method.Name}.\", statusCode: 500) : Ok(ret.Data);");
                    controllerContent.AppendLine($"            }}");
                    controllerContent.AppendLine($"            catch (Exception ex)");
                    controllerContent.AppendLine($"            {{");
                    controllerContent.AppendLine($"                _logger.LogError(ex, \"An unexpected error occurred while processing the request to get {method.Name}.\");");
                    controllerContent.AppendLine($"                return Problem(detail: \"An unexpected error occurred. Please try again later.\", statusCode: 500);");
                    controllerContent.AppendLine($"            }}");

                    IcontrollerContent.AppendLine($"        Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Name}Request request);");
                }
                else if (method.IsGet)
                {
                    if (method.Name == "GetIcons")
                    {
                    }

                    controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Parameter})");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            try");
                    controllerContent.AppendLine($"            {{");
                    controllerContent.AppendLine($"            DataResponse<{method.ReturnType}> ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    controllerContent.AppendLine($"             return ret.IsError ? Problem(detail: \"An unexpected error occurred retrieving {method.Name}.\", statusCode: 500) : Ok(ret.Data);");
                    controllerContent.AppendLine($"            }}");
                    controllerContent.AppendLine($"            catch (Exception ex)");
                    controllerContent.AppendLine($"            {{");
                    controllerContent.AppendLine($"                _logger.LogError(ex, \"An unexpected error occurred while processing the request to get {method.Name}.\");");
                    controllerContent.AppendLine($"                return Problem(detail: \"An unexpected error occurred. Please try again later.\", statusCode: 500);");
                    controllerContent.AppendLine($"            }}");

                    IcontrollerContent.AppendLine($"        Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Parameter});");
                }
                else if (method.IsTask)
                {
                    controllerContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.TsParameter} request)");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            {method.ReturnType} ret = await taskQueue{method.Name}(request).ConfigureAwait(false);");
                    IcontrollerContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                }
                else
                {

                    controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({parameterLine})");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send({toSend}).ConfigureAwait(false);");
                    if (method.IsReturnNull)
                    {
                        controllerContent.AppendLine($"            return ret == null ? NotFound(ret) : Ok(ret);");
                    }
                    else
                    {
                        controllerContent.AppendLine($"            return Ok(ret);");
                    }
                    IcontrollerContent.AppendLine($"        Task<ActionResult<{method.ReturnType}>> {method.Name}({parameterLine});");
                }

                controllerContent.AppendLine($"        }}");
                controllerContent.AppendLine();
            }

            // Hub method signature (if applicable)
            if (!method.JustController)
            {
                if (method.ReturnType.Equals("APIResponse?") || (method.IsGet && method.ReturnType.EndsWith("?")))
                {
                    method.ReturnType = method.ReturnType[..^1];
                }


                if (method.IsGetPaged)
                {
                    hubContent.AppendLine($"        public async Task<PagedResponse<{method.ReturnType}>> {method.Name}({method.Parameter})");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            PagedResponse<{method.ReturnType}> ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    hubContent.AppendLine($"            return ret;");
                    IhubContent.AppendLine($"        Task<PagedResponse<{method.ReturnType}>> {method.Name}({method.Parameter});");
                }
                else if (method.IsGetCached)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"             DataResponse<{method.ReturnType}> ret = await Sender.Send(request).ConfigureAwait(false);");
                    hubContent.AppendLine($"            return ret.Data;");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                }
                else if (method.IsGet)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Parameter})");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"             DataResponse<{method.ReturnType}> ret = await Sender.Send(new {method.SingalRFunction}({method.ParameterNames})).ConfigureAwait(false);");
                    hubContent.AppendLine($"            return ret.Data;");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Parameter});");
                }
                else if (method.IsTask)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            await taskQueue.{method.Name}(request).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                    hubContent.AppendLine($"            return APIResponse.Ok;");
                }
                else
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({parameterLine})");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send({toSend}).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({parameterLine});");
                    hubContent.AppendLine($"            return ret;");
                }

                hubContent.AppendLine($"        }}");
                hubContent.AppendLine();
            }
        }

        string assssa = controllerContent.ToString();

        WriteControllerAndHub(filePath, namespaceName, controllerContent, hubContent, needsQueryInclude, needsCommandInclude);
        WriteIControllerAndHub(IFilePath, namespaceName, IcontrollerContent, IhubContent, needsQueryInclude, needsCommandInclude);
    }

    private static void WriteIControllerAndHub(string IFilePath, string namespaceName, StringBuilder IControllerContent, StringBuilder IHubContent, bool NeedsQueryInclude, bool NeedsCommandInclude)
    {
        string fileContent = "using Microsoft.AspNetCore.Mvc;\n";
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

    private static void WriteControllerAndHub(string filePath, string namespaceName, StringBuilder controllerContent, StringBuilder hubContent, bool NeedsQueryInclude, bool NeedsCommandInclude)
    {
        string fileContent = "using Microsoft.AspNetCore.Mvc;\n";
        if (NeedsCommandInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Commands;\n";
        }

        if (NeedsQueryInclude)
        {
            fileContent += $"using StreamMaster.Application.{namespaceName}.Queries;\n";
        }

        string ns = namespaceName + ".Controllers";

        fileContent += $@"
namespace StreamMaster.Application.{ns}
{{
    public partial class {namespaceName}Controller(ILogger<{namespaceName}Controller> _logger) : ApiControllerBase, I{namespaceName}Controller
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
