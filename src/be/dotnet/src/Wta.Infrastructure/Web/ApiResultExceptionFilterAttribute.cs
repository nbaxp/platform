using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wta.Infrastructure.Web;

public class ApiResultExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(context.Exception.Message)
        {
            StatusCode = 500,
            Value = ApiResult.Create(context.Exception?.StackTrace, 500, context.Exception?.Message)
        };
    }
}
