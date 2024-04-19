namespace Wta.Infrastructure.Web;

public class ApiResult<T>
{
    public ApiResult()
    {
    }

    public ApiResult(T? data, int code = 0, string? message = null)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class ApiResult : ApiResult<object>
{
    public static ApiResult<T> Create<T>(T? data, int code = 0, string? message = null)
    {
        return new ApiResult<T>(data, code, message);
    }
}
