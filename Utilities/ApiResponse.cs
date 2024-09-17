using System.Net;

namespace GajinoAgencies.Utilities;

public static class ApiResponse
{
    public static ApiResponseDto<T>
        ResultMessage<T>(T? data = default, HttpStatusCode status = HttpStatusCode.OK, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Data = data,
            Status = status,
            Message = message
        };
    }
}

public class ApiResponseDto
{
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public object? Data { get; set; } = null;
    public string? Message { get; set; } = null;
}

public class ApiResponseDto<T>
{
    public void SetResult(T data, HttpStatusCode status, string message)
    {
        Data = data;
        Status = status;
        Message = message;
    }

    public void SetResult(HttpStatusCode status, string message)
    {
        Status = status;
        Message = message;
    }
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public T? Data { get; set; } = default;
    public string? Message { get; set; } = null;
}
