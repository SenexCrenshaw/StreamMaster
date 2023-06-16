using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace StreamMasterInfrastructure;

public class VersionedApiControllerAttribute : Attribute, IRouteTemplateProvider, IEnableCorsAttribute, IApiBehaviorMetadata
{
    public const string API_CORS_POLICY = "ApiCorsPolicy";
    public const string CONTROLLER_RESOURCE = "[controller]";

    public VersionedApiControllerAttribute(int version, string resource = CONTROLLER_RESOURCE)
    {
        Resource = resource;
        //Template = $"api/v{version}/{resource}";
        Template = $"{resource}";
        //Template = "api/ChannelGroups";
        PolicyName = API_CORS_POLICY;
    }

    public string Name { get; set; }
    public int? Order => 2;
    public string PolicyName { get; set; }
    public string Resource { get; }
    public string Template { get; }
}

public class V1ApiControllerAttribute : VersionedApiControllerAttribute
{
    public V1ApiControllerAttribute(string resource = "[controller]")
        : base(1, resource)
    {
    }
}
