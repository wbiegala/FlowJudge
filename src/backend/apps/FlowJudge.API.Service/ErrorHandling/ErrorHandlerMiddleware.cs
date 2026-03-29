using FlowJudge.Common.Http;
using FlowJudge.Common.Http.Extensions;

namespace FlowJudge.API.Service.ErrorHandling
{
    internal sealed class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception was thrown while processing request id={requestId}!", GetRequestId(context));

                var errorResponseData = ex.ToProblemDetails();

                context.Response.ContentType = errorResponseData.ContentType;
                context.Response.StatusCode = errorResponseData.ErrorCode;
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponseData.ErrorBody);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private static string GetRequestId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(RequestMetadataHeaders.OperationIdHeader, out var requestId))
            {
                return requestId.ToString();
            }

            return context.TraceIdentifier;
        }
    }
}
