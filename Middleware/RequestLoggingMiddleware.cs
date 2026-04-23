using System.Diagnostics;
namespace ClinicalTrialsApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;
        _logger.LogInformation("Handling -> {Method} {Path} -- starting", method, path);

        try
        {
            await _next(context); // invoca el siguiente middleware
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("<- {Method} {Path} -- {StatusCode} in {ElapsedMilliseconds} ms", method, path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}