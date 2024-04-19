using Microsoft.AspNetCore.Mvc;

namespace Wta.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseController : ControllerBase
{
    protected ApiResult<T> Json<T>(T data, int code = 0, string? message = null)
    {
        return new ApiResult<T> { Data = data, Code = code, Message = message };
    }
}
