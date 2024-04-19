namespace Wta.Application.Default.Controllers;

public class MenuController(IRepository<Permission> menuRepository) : BaseController
{
    [Route("/api/menu")]
    [HttpPost]
    [AllowAnonymous]
    public ApiResult<object> Menu()
    {
        var result = menuRepository.AsNoTracking().OrderBy(o => o.Order).ToList();
        return Json(result as object);
    }
}
