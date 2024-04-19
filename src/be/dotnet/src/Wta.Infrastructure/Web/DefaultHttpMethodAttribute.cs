using Microsoft.AspNetCore.Mvc.Routing;

namespace Wta.Infrastructure.Web;

public class DefaultHttpMethodAttribute(IEnumerable<string> httpMethods) : HttpMethodAttribute(httpMethods)
{
}
