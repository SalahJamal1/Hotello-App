using System.Net;
using MyApplication.Exceptions;
using Newtonsoft.Json;

namespace MyApplication.Middleware;

public class ExceptionMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Something went While processing  {context.Request.Path}");
            await HandleExceptionAsync(context, e);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var errorDetails = new ErrorDetails
        {
            ErrorType = "failure",
            ErrorMessage = ex.Message
        };

        switch (ex)
        {
            case AppErrorException appError:
                statusCode = HttpStatusCode.BadRequest;
                break;
        }

        var response = JsonConvert.SerializeObject(errorDetails);
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(response);
    }
}