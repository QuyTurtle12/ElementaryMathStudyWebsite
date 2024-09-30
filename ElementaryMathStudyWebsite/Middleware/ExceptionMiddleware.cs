using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json;

namespace ElementaryMathStudyWebsite.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle BadRequestException specifically
                context.Response.StatusCode = badRequestEx.StatusCode;
                var result = JsonSerializer.Serialize(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle NotFoundException specifically
                context.Response.StatusCode = notFoundEx.StatusCode;
                var result = JsonSerializer.Serialize(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle CoreException specifically
                context.Response.StatusCode = coreEx.StatusCode;
                var result = JsonSerializer.Serialize(new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            catch (BaseException.UnauthorizedException unAuthEx)
            {
                // Handle NotFoundException specifically
                context.Response.StatusCode = unAuthEx.StatusCode;
                var result = JsonSerializer.Serialize(new
                {
                    errorCode = unAuthEx.ErrorDetail.ErrorCode,
                    errorMessage = unAuthEx.ErrorDetail.ErrorMessage
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                // Handle all other unexpected exceptions
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var result = JsonSerializer.Serialize(new
                {
                    error = $"An unexpected error occurred. Detail: {ex.Message}"
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }

        }
    }
}
