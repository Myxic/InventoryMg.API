using InventoryMg.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;


namespace InventoryMg.DAL.Configurations
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            var stackTrace = string.Empty;
            string message = string.Empty;

            var exceptionType = ex.GetType();
            if (exceptionType == typeof(BLL.Exceptions.BadRequestException))
            {
                message = ex.Message;
                status = HttpStatusCode.BadRequest;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(BLL.Exceptions.NotFoundException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotFound;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(BLL.Exceptions.NotImplementedException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotImplemented;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(BLL.Exceptions.UnauthorizedAccessException))
            {
                message = ex.Message;
                status = HttpStatusCode.Unauthorized;
                stackTrace = ex.StackTrace;
            }
            else if (exceptionType == typeof(BLL.Exceptions.KeyNotFoundException))
            {
                message = ex.Message;
                status = HttpStatusCode.NotFound;
                stackTrace = ex.StackTrace;
            }
            else
            {
                message = ex.Message;
                status = HttpStatusCode.InternalServerError;
                stackTrace = ex.StackTrace;
            }
            var exceptionResult = JsonSerializer.Serialize(new { error = message, stackTrace });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(exceptionResult);
        }

    }
}
