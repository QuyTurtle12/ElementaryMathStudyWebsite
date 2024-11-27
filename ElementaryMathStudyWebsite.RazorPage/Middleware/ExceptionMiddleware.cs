using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json;

namespace ElementaryMathStudyWebsite.RazorPage.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

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
            catch (BaseException.BadRequestException badRequestEx)
            {
                _logger.LogError(badRequestEx, "BadRequestException occurred.");
                await HandleExceptionAsync(context, badRequestEx.StatusCode, new
                {
                    type = "error",
                    message = "Bad Request",
                    description = badRequestEx.ErrorDetail.ErrorMessage,
                    errorCode = badRequestEx.ErrorDetail.ErrorCode
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                _logger.LogError(notFoundEx, "NotFoundException occurred.");
                await HandleExceptionAsync(context, notFoundEx.StatusCode, new
                {
                    type = "error",
                    message = "Not Found",
                    description = notFoundEx.ErrorDetail.ErrorMessage,
                    errorCode = notFoundEx.ErrorDetail.ErrorCode
                });
            }
            catch (BaseException.CoreException coreEx)
            {
                _logger.LogError(coreEx, "CoreException occurred.");
                await HandleExceptionAsync(context, coreEx.StatusCode, new
                {
                    type = "error",
                    message = coreEx.Message,
                    description = coreEx.AdditionalData,
                    code = coreEx.Code
                });
            }
            catch (BaseException.UnauthorizedException unAuthEx)
            {
                _logger.LogError(unAuthEx, "UnauthorizedException occurred.");
                await HandleExceptionAsync(context, unAuthEx.StatusCode, new
                {
                    type = "error",
                    message = "Unauthorized",
                    description = unAuthEx.ErrorDetail.ErrorMessage,
                    errorCode = unAuthEx.ErrorDetail.ErrorCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, new
                {
                    type = "error",
                    message = "Internal Server Error",
                    description = ex.Message
                });
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, object response)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}