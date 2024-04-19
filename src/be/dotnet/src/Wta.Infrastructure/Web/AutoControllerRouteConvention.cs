using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Wta.Infrastructure.Controllers;

public class AutoControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsAssignableTo(typeof(Controller)))
        {
            return;
        }
        var routeTemplate = $"api/[controller]/[action]";
        if (!controller.Selectors.Any(o => o.AttributeRouteModel != null))
        {
            controller.Selectors.First().AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate));
        }
        controller.Actions.ForEach(action =>
        {
            if (!action.Attributes.Any(o => o.GetType().IsAssignableTo(typeof(HttpMethodAttribute))))
            {
                var match = Regex.Match(action.ActionName, "^(Get|Post|Put|Delete|Patch|Head|Options)", RegexOptions.IgnoreCase);
                var method = match.Success ? match.Groups[1].Value.ToUpperInvariant() : "POST";
                (action.Attributes as List<object>)?.Add(new Web.DefaultHttpMethodAttribute(new List<string> { method }));
                action.Selectors.First().ActionConstraints.Add(new HttpMethodActionConstraint(new List<string> { method }));
            }
        });
        if (controller.ApiExplorer.GroupName == null)
        {
            if (!controller.ControllerType.IsGenericType || controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericController<,>))
            {
                controller.ApiExplorer.GroupName = controller.ControllerType.Assembly.GetName().Name;
            }
        }
    }
}
