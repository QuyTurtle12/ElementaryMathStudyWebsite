using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json;

namespace ElementaryMathStudyWebsite.Middleware
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

        public async Task Invoke(HttpContext context)
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
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                _logger.LogError(notFoundEx, "NotFoundException occurred.");
                await HandleExceptionAsync(context, notFoundEx.StatusCode, new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.CoreException coreEx)
            {
                _logger.LogError(coreEx, "CoreException occurred.");
                await HandleExceptionAsync(context, coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.UnauthorizedException unAuthEx)
            {
                _logger.LogError(unAuthEx, "UnauthorizedException occurred.");
                await HandleExceptionAsync(context, unAuthEx.StatusCode, new
                {
                    errorCode = unAuthEx.ErrorDetail.ErrorCode,
                    errorMessage = unAuthEx.ErrorDetail.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, new
                {
                    error = $"An unexpected error occurred. Detail: {ex.Message}"
                });
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, object result)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}
