using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ElementaryMathStudyWebsite.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private List<string> IgnoreSegments = [];
        private List<string> IgnoreContentType = [];
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            InitIgnoreContentType();
        }
        public async Task Invoke(HttpContext context)
        {
            if (IgnoreSegments.Any(x => context.Request.Path.StartsWithSegments(x))
               || context.Request.Method.ToLower().Equals("get")
               || context.Request.Host.ToString().Contains("localhost"))
            {
                await _next(context);
            }
            else
            {
                EndpointMetadataCollection? endpointMetaData = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata;

                context.Request.EnableBuffering();

                AuditLog auditLog = await LogRequest(context);
                await LogResponse(context, auditLog);
            }
        }
        private void InitIgnoreContentType()
        {
            // type application
            IgnoreContentType.Add("application/java-archive");
            IgnoreContentType.Add("application/EDI-X12");
            IgnoreContentType.Add("application/EDIFACT");
            IgnoreContentType.Add("application/javascript");
            IgnoreContentType.Add("application/octet-stream");
            IgnoreContentType.Add("application/ogg");
            IgnoreContentType.Add("application/pdf");
            IgnoreContentType.Add("application/xhtml+xml");
            IgnoreContentType.Add("application/x-shockwave-flash");
            // IgnoreContentType.Add("application/json");
            // IgnoreContentType.Add("application/ld+json");
            // IgnoreContentType.Add("application/xml");
            IgnoreContentType.Add("application/zip");
            // IgnoreContentType.Add("application/x-www-form-urlencoded");

            // type audio
            IgnoreContentType.Add("audio/mpeg");
            IgnoreContentType.Add("audio/x-ms-wma");
            IgnoreContentType.Add("audio/vnd.rn-realaudio");
            IgnoreContentType.Add("audio/x-wav");

            // type image
            IgnoreContentType.Add("image/gif");
            IgnoreContentType.Add("image/jpeg");
            IgnoreContentType.Add("image/png");
            IgnoreContentType.Add("image/tiff");
            IgnoreContentType.Add("image/vnd.microsoft.icon");
            IgnoreContentType.Add("image/x-icon");
            IgnoreContentType.Add("image/vnd.djvu");
            IgnoreContentType.Add("image/svg+xml");

            // type multipart
            IgnoreContentType.Add("multipart/mixed");
            IgnoreContentType.Add("multipart/alternative");
            IgnoreContentType.Add("multipart/related");
            IgnoreContentType.Add("multipart/form-data");

            // type text
            IgnoreContentType.Add("text/css");
            IgnoreContentType.Add("text/csv");
            IgnoreContentType.Add("text/html");
            IgnoreContentType.Add("text/javascript");
            IgnoreContentType.Add("text/plain");
            IgnoreContentType.Add("text/xml");

            // type video
            IgnoreContentType.Add("video/mpeg");
            IgnoreContentType.Add("video/mp4");
            IgnoreContentType.Add("video/quicktime");
            IgnoreContentType.Add("video/x-ms-wmv");
            IgnoreContentType.Add("video/x-msvideo");
            IgnoreContentType.Add("video/x-flv");
            IgnoreContentType.Add("video/webm");

            // type vnd (?)
            IgnoreContentType.Add("application/vnd.android.package-archive");
            IgnoreContentType.Add("application/vnd.oasis.opendocument.text");
            IgnoreContentType.Add("application/vnd.oasis.opendocument.spreadsheet");
            IgnoreContentType.Add("application/vnd.oasis.opendocument.presentation");
            IgnoreContentType.Add("application/vnd.oasis.opendocument.graphics");
            IgnoreContentType.Add("application/vnd.ms-excel");
            IgnoreContentType.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            IgnoreContentType.Add("application/vnd.ms-powerpoint");
            IgnoreContentType.Add("application/vnd.openxmlformats-officedocument.presentationml.presentation");
            IgnoreContentType.Add("application/msword");
            IgnoreContentType.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            IgnoreContentType.Add("application/vnd.mozilla.xul+xml");
        }
        public async Task LogResponse(HttpContext context, AuditLog auditLog)
        {
            if (auditLog == null)
            {
                await _next(context);
                return;
            }

            Stream originalBody = context.Response.Body;

            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                auditLog.UserId = context.User.Claims.FirstOrDefault(v => v.Type == ClaimTypes.NameIdentifier)?.Value;
                auditLog.UserName = context.User.Claims.FirstOrDefault(v => v.Type == ClaimTypes.Name)?.Value;
                auditLog.Claims = string.Join(',', context.User.Claims.Select(v => $"{v.Type}:{v.Value}"));
            }

            try
            {

                using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                await _next(context);

                memStream.Position = 0;
                string responseBody = new StreamReader(memStream).ReadToEnd();

                auditLog.ResponseStatusCode = context.Response.StatusCode.ToString();
                auditLog.ResponseBody = responseBody;

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            if (context.Response.StatusCode == 404
               || context.Response.StatusCode == 500
               || context.Response.StatusCode == 401
               || context.Response.StatusCode == 403
               || context.Response.StatusCode == 502
               || IgnoreContentType.Contains(context.Response.ContentType!))
            {
                await _next(context);
                return;
            }

            //await unitOfWork.GetRepository<AuditLog>().InsertAsync(auditLog);
            //await unitOfWork.SaveAsync();
        }
        public async Task<AuditLog> LogRequest(HttpContext context)
        {
            string requestUri = context.Request.Path.Value!;
            IFormCollection? form = null;
            string formString = string.Empty;

            if (context.Request.HasFormContentType)
            {
                form = context.Request.Form;
            }
            else
            {
                formString = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var injectedRequestStream = new MemoryStream();
                byte[] bytesToWrite = Encoding.UTF8.GetBytes(formString);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = injectedRequestStream;
            }
            AuditLog log = new()
            {
                CreatedTime = DateTimeOffset.Now,
                ResQuestBody = formString,
                RemoteHost = context.Connection.RemoteIpAddress?.ToString(),
                HttpURL = requestUri,
                LocalAddress = context.Connection.LocalIpAddress?.ToString(),
                Headers = JsonSerializer.Serialize(context.Request.Headers),
                Form = form != null ? JsonSerializer.Serialize(form) : ""
            };
            return log;
        }
    }
}
