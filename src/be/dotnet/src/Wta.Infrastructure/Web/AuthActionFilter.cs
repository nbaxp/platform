using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Wta.Infrastructure.Exceptions;

namespace Wta.Infrastructure.Web;

public class AuthActionFilter(IConfiguration configuration) : IActionFilter
{
    /// <summary>
    /// 200\400\500返回值规范化为CustomApiResponse
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            if (context.Exception is BadRequestException badRequestException)
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = 400,
                    Value = ApiResult.Create(context.ModelState.ToErrors(), badRequestException.Code, context.Exception?.Message)
                };
            }
            else if (context.Exception is ProblemException problemException)
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = 500,
                    Value = ApiResult.Create(context.Exception?.StackTrace, problemException.Code, context.Exception?.Message)
                };
            }
            else
            {
                context.Result = new ObjectResult(context.Exception.Message)
                {
                    StatusCode = 500,
                    Value = ApiResult.Create(context.Exception?.StackTrace, 500, context.Exception?.Message)
                };
            }
            context.ExceptionHandled = true;
        }
        else
        {
            if (context.Result is ObjectResult result && result.Value is ProblemDetails problemDetails)
            {
                result.Value = ApiResult.Create(problemDetails.Extensions, 500, problemDetails.Detail);
            }
            else
            {
                if (context.Result is BadRequestResult)
                {
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                else if (context.Result is ObjectResult objectResult)
                {
                    if (!objectResult.Value!.GetType().IsGenericType ||
                        objectResult.Value.GetType().GetGenericTypeDefinition() != typeof(ApiResult<>))
                    {
                        context.Result = new JsonResult(ApiResult.Create(objectResult.Value));
                    }
                }
                else
                {
                    //其他类型
                }
                if (context.Result is BadRequestObjectResult badRequestObjectResult)
                {
                    badRequestObjectResult.Value = ApiResult.Create(context.ModelState.ToErrors(), 400);
                }
            }
        }
    }

    /// <summary>
    /// 权限验证
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (configuration.GetValue("SkipAuth", false))
        {
            return;
        }
        var requireAuth = false;
        var descriptor = (context.ActionDescriptor as ControllerActionDescriptor)!;
        var controllerData = descriptor.ControllerTypeInfo.CustomAttributes;
        var actionData = descriptor.MethodInfo.CustomAttributes;
        if (controllerData.Any(o => o.AttributeType == typeof(AuthorizeAttribute)))
        {
            if (!actionData.Any(o => o.AttributeType == typeof(AllowAnonymousAttribute)))
            {
                requireAuth = true;
            }
        }
        else
        {
            if (controllerData.Any(o => o.AttributeType == typeof(AllowAnonymousAttribute)))
            {
                if (actionData.Any(o => o.AttributeType == typeof(AuthorizeAttribute)))
                {
                    requireAuth = true;
                }
            }
            else if (!actionData.Any(o => o.AttributeType == typeof(AllowAnonymousAttribute)))
            {
                requireAuth = true;
            }
        }
        if (requireAuth)
        {
            if (!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                var valid = false;
                var roles = (controllerData.FirstOrDefault(o => o.AttributeType == typeof(AuthorizeAttribute))?.NamedArguments.FirstOrDefault() ??
                    actionData.FirstOrDefault(o => o.AttributeType == typeof(AuthorizeAttribute))?
                    .NamedArguments.FirstOrDefault())?.TypedValue.Value?.ToString();
                if (!string.IsNullOrEmpty(roles))
                {
                    var roleList = roles.Split(',');
                    var roleType = nameof(ClaimTypes.Role).ToLowerInvariant();
                    valid = context.HttpContext.User.Claims.Any(o => o.Type == roleType && roleList.Contains(o.Value));
                }
                else
                {
                    if (controllerData.Any(o => o.AttributeType == typeof(AuthorizeAttribute)) ||
                        actionData.Any(o => o.AttributeType == typeof(AuthorizeAttribute)))
                    {
                        valid = true;
                    }
                    else
                    {
                        var operation = $"{descriptor.ControllerName}.{descriptor.ActionName}";
                        valid = context.HttpContext.User.IsInRole(operation);
                    }
                }
                if (!valid)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
