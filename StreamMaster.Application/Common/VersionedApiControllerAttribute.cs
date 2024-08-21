using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace StreamMaster.Application.Common;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class VersionedApiControllerAttribute(int version, string resource = VersionedApiControllerAttribute.CONTROLLER_RESOURCE) : Attribute, IRouteTemplateProvider, IEnableCorsAttribute, IApiBehaviorMetadata
{
    public const string API_CORS_POLICY = "ApiCorsPolicy";
    public const string CONTROLLER_RESOURCE = "[controller]";

    public string Name { get; set; } = string.Empty;
    public int? Order => 2;
    public string? PolicyName { get; set; } = API_CORS_POLICY;
    public string Resource { get; } = resource;
    public string Template { get; } = $"{resource}";
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class V1ApiControllerAttribute(string resource = "[controller]") : VersionedApiControllerAttribute(1, resource)
{
}
