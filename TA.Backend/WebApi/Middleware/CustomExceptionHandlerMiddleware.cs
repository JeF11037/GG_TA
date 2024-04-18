using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using WebApi.Contracts;

namespace WebApi.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next) =>
            _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var api_error = new ErrorResponse
            {
                StatusCode = (int)code,
                ErrorMessage = "Internal Server Error",
                Timestamp = DateTime.Now
            };

            api_error.Errors.Add(exception.ToString());

            if (result == string.Empty)
            {
                result = JsonSerializer.Serialize(new { errors = api_error });
            }

            return context.Response.WriteAsync(result);
        }
    }
}
