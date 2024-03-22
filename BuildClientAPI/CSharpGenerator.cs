using System.Text;

public static class CSharpGenerator
{
    public static void GenerateFile(string namespaceName, List<MethodDetails> methods, string filePath)
    {
        StringBuilder controllerContent = new();
        StringBuilder hubContent = new();

        string serviceName = $"{namespaceName}Service";
        string serviceNameParameter = serviceName;

        // Generate controller and hub content based on methods
        foreach (MethodDetails method in methods)
        {
            // Determine HTTP method attribute (simplified example)
            string httpAttribute = "HttpPut";
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

            string fromQuery = method.Name.StartsWith("Get") ? "[FromQuery] " : "";
            string route = $"[Route(\"[action]\")]";
            string httpMethodLine = $"[{httpAttribute}]";

            if (method.Name == "AddSMStreamToSMChannel")
            {
                int a = 1;
            }

            controllerContent.AppendLine($"        {httpMethodLine}");
            controllerContent.AppendLine($"        {route}");

            if (method.Name.StartsWith("Get"))
            {
                controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({fromQuery}{method.Parameters})");
                controllerContent.AppendLine($"        {{");
                controllerContent.AppendLine($"            {method.ReturnType} ret = await Sender.Send(new {method.Name}({method.ParameterNames})).ConfigureAwait(false);");
                controllerContent.AppendLine($"            return ret;");
            }
            else
            {
                controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({fromQuery}{method.Name} request)");
                controllerContent.AppendLine($"        {{");
                controllerContent.AppendLine($"            {method.ReturnType}? ret = await Sender.Send(request).ConfigureAwait(false);");
                controllerContent.AppendLine($"            return ret == null ? NotFound() : Ok(ret);");
            }

            controllerContent.AppendLine($"        }}");
            controllerContent.AppendLine();

            // Hub method signature (if applicable)
            if (method.IncludeInHub)
            {
                hubContent.AppendLine($"        public async Task<{method.ReturnType}?> {method.Name}({method.Name} request)");
                hubContent.AppendLine($"        {{");
                hubContent.AppendLine($"            {method.ReturnType}? ret = await Sender.Send(request).ConfigureAwait(false);");
                hubContent.AppendLine($"            return ret;");
                hubContent.AppendLine($"        }}");
                hubContent.AppendLine();
            }
        }

        // Assemble the full file content with namespaces and class definitions
        string fileContent = $@"using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.{namespaceName}.Commands;

namespace StreamMaster.Application.{namespaceName}
{{
    public partial class {namespaceName}Controller(ISender Sender) : ApiControllerBase
    {{        

{controllerContent}    }}
}}

namespace StreamMaster.Application.Hubs
{{
    public partial class StreamMasterHub 
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
