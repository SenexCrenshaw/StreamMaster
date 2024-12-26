using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace StreamMaster.API;

public class RoutePrefixConvention(string routePrefix) : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        {
            foreach (SelectorModel selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel.Template = $"{routePrefix}/{selector.AttributeRouteModel.Template}";
                }
            }
        }
    }
}

public class GlobalRoutePrefixConvention(string prefix) : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        {
            foreach (SelectorModel selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    // Prepend the prefix to existing routes
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new RouteAttribute(prefix)),
                        selector.AttributeRouteModel);
                }
            }
        }
    }
}
