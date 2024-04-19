namespace Wta.Application.Default.Controllers;

public class ExampleController : BaseController
{
    [Authorize]
    public string Test1()
    {
        return "OK";
    }

    [Authorize(Roles = "admin")]
    public ApiResult<string> Test2()
    {
        return Json("OK");
    }

    [AllowAnonymous]
    public object Test3()
    {
        return new
        {
            AB = 1,
            A_B = 2,
            Ab_Cd = 3,
            AbCd_EfGh = 4
        };
    }
}
