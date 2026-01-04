using Ecommerce.API.Utils;
using System.Net;
using System.Text.Json;

namespace Ecommerce.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (ErrorHandler ex)
            {
                _logger.LogError(ex, "Business logic error");
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = exception is ErrorHandler errorHandler
                ? errorHandler.StatusCode
                : (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                success = false,
                error = exception.Message,
                statusCode = (int)code
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
