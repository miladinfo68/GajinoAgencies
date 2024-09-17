using System.Data;
using System.Net;
using System.Text;
using FluentValidation;

namespace GajinoAgencies.Utilities;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.Message ?? "");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception? exception)
    {
        _logger.LogError(exception, "An unexpected error occurred.");


        var expResponse = exception switch
        {
            UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, "Unauthorized: No token provided.."),
            ApplicationException exp => new ExceptionResponse(HttpStatusCode.BadRequest, exp.Message ?? "Application exception occurred."),
            ArgumentException exp => new ExceptionResponse(HttpStatusCode.BadRequest, exp.Message ?? "Application exception occurred."),
            KeyNotFoundException exp => new ExceptionResponse(HttpStatusCode.NotFound, exp.Message ?? "Requested Item was not found!"),
            DuplicateNameException exp => new ExceptionResponse(HttpStatusCode.Conflict, exp.Message ?? "Item already exist!"),
            SmsSendingException exp => new ExceptionResponse(exp.ErrorCode , exp.Message ?? "Error in sms provider side"),
            ForbiddenAccessException exp => new ExceptionResponse(HttpStatusCode.Forbidden, exp.Message ?? "Access Deny"),
            ValidationException exp => new ExceptionResponse(HttpStatusCode.BadRequest, GetValidationExceptionErrors(exp)),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error")
        };

        var responseObj = ApiResponse.ResultMessage<string>(null,expResponse.StatusCode,expResponse.Description);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)expResponse.StatusCode;
        await context.Response.WriteAsJsonAsync(responseObj);

        //var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseObj);
        //await context.Response.WriteAsJsonAsync(jsonResponse);
    }

    private record ExceptionResponse(HttpStatusCode StatusCode, string Description);

    private static string GetValidationExceptionErrors(Exception exception)
    {
        var sb = new StringBuilder();
        if (exception is ValidationException validationException && validationException.Errors?.Count() > 0)
        {
            foreach (var err in validationException.Errors)
            {
                //sb.Append($"{err.PropertyName}-{err.ErrorMessage}\r\n");
                sb.Append($"{err.ErrorMessage}");
                sb.Append("\r\n");
            }
        }
        var res = sb.ToString();
        return res;
    }
}