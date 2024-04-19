using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Wta.Application.Default.Controllers;

public class FeaturesController : ControllerBase
{
    private readonly ApplicationPartManager _partManager;

    public FeaturesController(ApplicationPartManager partManager)
    {
        _partManager = partManager;
    }

    public IActionResult Index()
    {
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);
        var tagHelperFeature = new TagHelperFeature();
        _partManager.PopulateFeature(tagHelperFeature);
        var viewComponentFeature = new ViewComponentFeature();
        _partManager.PopulateFeature(viewComponentFeature);
        return Ok(new
        {
            Controllers = controllerFeature.Controllers.Select(o => new { o.Name }).ToList(),
            TagHelpers = tagHelperFeature.TagHelpers.Select(o => new { o.Name }).ToList(),
            ViewComponents = viewComponentFeature.ViewComponents.Select(o => new { o.Name }).ToList(),
        });
    }
}
