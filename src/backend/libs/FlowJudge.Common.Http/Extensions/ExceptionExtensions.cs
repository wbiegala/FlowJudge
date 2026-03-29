using FlowJudge.Common.Http.ErrorHandling;
using Microsoft.AspNetCore.Http;

namespace FlowJudge.Common.Http.Extensions
{
    public static class ExceptionExtensions
    {
        private const string DefaultErrorCode = "api.internal_server_error";

        public static ExceptionProblemMetadata ToProblemDetails(this Exception exception)
        {
            return new ExceptionProblemMetadata(
                ContentTypes.ApplicationJson,
                StatusCodes.Status500InternalServerError,
                new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Title = DefaultErrorCode,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://flowjudge.com/errors/" + DefaultErrorCode,
                    Detail = exception.Message
                });
        }

        public static Microsoft.AspNetCore.Mvc.ProblemDetails ToProblemDetails(this Exception exception, int statusCode, string errorCode)
        {
            return new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = errorCode,
                Status = statusCode,
                Type = "https://flowjudge.com/errors/" + errorCode,
                Detail = exception.Message
            };
        }
    }
}
