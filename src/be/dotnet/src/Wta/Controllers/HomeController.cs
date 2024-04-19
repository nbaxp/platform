using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wta.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    [ResponseCache(NoStore = true)]
    public IActionResult Index()
    {
        return File("~/index.html", "text/html");
    }
}
