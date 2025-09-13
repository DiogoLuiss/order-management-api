using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace OrderManagementApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Constructors

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorMessage = GetErrorMessages(exception);
            var errorResponse = new { messages = errorMessage };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(jsonResponse);
        }

        private string[] GetErrorMessages(Exception exception)
        {
            // Add custom logic for different exception types if needed
            return new string[] { exception.Message };
        }

        #endregion
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
