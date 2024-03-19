using System.Text;

public static class CSharpGenerator
{
    public static void GenerateFile(
        string namespaceName,
        List<MethodDetails> methods,
        string filePath)
    {
        StringBuilder controllerContent = new();
        StringBuilder hubContent = new();

        string serviceName = $"{namespaceName}Service";
        string serviceNameParameter = serviceName;// char.ToLowerInvariant(serviceName[0]) + serviceName[1..]; // Convert first letter to lowercase

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
            else if (method.Name.StartsWith("Create"))
            {
                httpAttribute = "HttpPost";
            }


            //string httpAttribute = method.Name.StartsWith("Get") ? "HttpGet" : "HttpPut";
            string fromQuery = method.Name.StartsWith("Get") ? "[FromQuery] " : "";
            string route = $"[Route(\"[action]\")]";
            string httpMethodLine = $"[{httpAttribute}]";

            if (method.Name == "GetPagedSMChannels")
            {
                int a = 1;
            }
            // Controller method signature
            controllerContent.AppendLine($"        {httpMethodLine}");
            controllerContent.AppendLine($"        {route}");
            controllerContent.AppendLine($"        public async Task<ActionResult<{method.ReturnType}>> {method.Name}({fromQuery}{method.Parameters})");
            controllerContent.AppendLine($"        {{");
            controllerContent.AppendLine($"            {method.ReturnType} ret = await {serviceNameParameter}.{method.Name}({method.ParameterNames}).ConfigureAwait(false);");
            controllerContent.AppendLine($"            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);");
            controllerContent.AppendLine($"        }}");
            controllerContent.AppendLine();

            // Hub method signature (if applicable)
            if (method.IncludeInHub)
            {
                hubContent.AppendLine($"        public async Task<{method.ReturnType}> {method.Name}({method.Parameters})");
                hubContent.AppendLine($"        {{");
                hubContent.AppendLine($"            {method.ReturnType} ret = await {serviceNameParameter}.{method.Name}({method.ParameterNames}).ConfigureAwait(false);");
                hubContent.AppendLine($"            return ret;");
                hubContent.AppendLine($"        }}");
                hubContent.AppendLine();
            }
        }

        // Assemble the full file content with namespaces and class definitions
        string fileContent = $@"using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.{namespaceName};

namespace StreamMaster.Application.{namespaceName}
{{
    public partial class {namespaceName}Controller(I{serviceName} {serviceName}) : ApiControllerBase, I{namespaceName}Controller
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
