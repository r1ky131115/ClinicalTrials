using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrialsApi.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;            
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // 1. Logueamos la excepción con todo el detalle
            _logger.LogError(exception, "Exception no manejada {Message} en {Path}", exception.Message, httpContext.Request.Path);

            // 2. Determinamos qué devolver según el tipo de excepción
            var (statusCode, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Argumento inválido"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Operación inválida"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            // 3. Creamos un ProblemDetails con la información del error
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = httpContext.Request.Path
            };

            // 4. Solo exponemos detalles técnicos en Development
            if (_env.IsDevelopment())
            {
                problemDetails.Detail = exception.Message;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            // 5. TraceId para correlacionar logs
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            // 6. Devolvemos la respuesta con el ProblemDetails
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Indica que la excepción fue manejada
        }
    }
}