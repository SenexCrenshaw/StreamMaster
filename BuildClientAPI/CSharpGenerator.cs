using System.Text;

public static class CSharpGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath, string IFilePath)
    {
        StringBuilder IcontrollerContent = new();
        StringBuilder controllerContent = new();

        StringBuilder IhubContent = new();
        StringBuilder hubContent = new();

        // Generate controller and hub content based on methods
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

            if (!method.JustHub)
            {
                controllerContent.AppendLine($"        {httpMethodLine}");
                controllerContent.AppendLine($"        {route}");

                if (method.IsGet)
                {
                    controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}([FromQuery] {method.Parameters})");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send(new {method.Name}({method.ParameterNames})).ConfigureAwait(false);");
                    controllerContent.AppendLine($"            return ret;");
                    IcontrollerContent.AppendLine($"    Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Parameters});");
                }
                else if (method.IsTask)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            {method.ReturnType} ret = await taskQueue{method.Name}(request).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                }
                else
                {
                    controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Name}Request request)");
                    controllerContent.AppendLine($"        {{");
                    controllerContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send(request).ConfigureAwait(false);");
                    controllerContent.AppendLine($"            return ret == null ? NotFound(ret) : Ok(ret);");
                    IcontrollerContent.AppendLine($"    Task<ActionResult<{method.ReturnType}>> {method.Name}({method.Name}Request request);");
                }

                controllerContent.AppendLine($"        }}");
                controllerContent.AppendLine();
            }

            // Hub method signature (if applicable)
            if (!method.JustController)
            {
                if (method.IsGet)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Parameters})");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send(new {method.Name}({method.ParameterNames})).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Parameters});");
                    hubContent.AppendLine($"            return ret;");
                }
                else if (method.IsTask)
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            await taskQueue.{method.Name}(request).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                    hubContent.AppendLine($"            return APIResponseFactory.Ok;");
                }
                else
                {
                    hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Name}Request request)");
                    hubContent.AppendLine($"        {{");
                    hubContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send(request).ConfigureAwait(false);");
                    IhubContent.AppendLine($"        Task<{method.ReturnType}> {method.Name}({method.Name}Request request);");
                    hubContent.AppendLine($"            return ret;");
                }

                hubContent.AppendLine($"        }}");
                hubContent.AppendLine();
            }
        }

        WriteControllerAndHub(filePath, namespaceName, controllerContent, hubContent);
        WriteIControllerAndHub(IFilePath, namespaceName, IcontrollerContent, IhubContent);
    }

    private static void WriteIControllerAndHub(string IFilePath, string namespaceName, StringBuilder IControllerContent, StringBuilder IHubContent)
    {

        // Assemble the full file content with namespaces and class definitions
        string fileContent = $@"using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.{namespaceName}.Commands;

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
        string directory = Directory.GetParent(IFilePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(IFilePath, fileContent);
    }

    private static void WriteControllerAndHub(string filePath, string namespaceName, StringBuilder controllerContent, StringBuilder hubContent)
    {
        // Assemble the full file content with namespaces and class definitions
        string fileContent = $@"using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.{namespaceName}.Commands;

namespace StreamMaster.Application.{namespaceName}
{{
    public partial class {namespaceName}Controller(ISender Sender) : ApiControllerBase, I{namespaceName}Controller
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
        string directory = Directory.GetParent(filePath).ToString();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, fileContent);
    }
}
