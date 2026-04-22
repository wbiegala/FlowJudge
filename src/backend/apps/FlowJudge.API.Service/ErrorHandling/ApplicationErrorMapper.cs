using FlowJudge.Common.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlowJudge.API.Service.ErrorHandling
{
    public static class ApplicationErrorMapper
    {
        public static ProblemDetails ToProblemDetails(this IError error, HttpStatusCode httpCode = HttpStatusCode.InternalServerError)
        {
            var problemDetails = new ProblemDetails
            {
                Title = error.Code,
                Type = $"https://httpstatuses.com/{(int)httpCode}",
                Status = (int)httpCode,
                Detail = error.Message,
                Extensions = error.Properties
            };

            return problemDetails;
        }

        public static IActionResult ToResponse(this IError error, HttpStatusCode httpCode = HttpStatusCode.InternalServerError)
        {
            var problemDetails = error.ToProblemDetails(httpCode);

            return problemDetails.ToResponse();
        }

        public static IActionResult ToResponse(this ProblemDetails errorModel)
        {
            var objectResult = new ObjectResult(errorModel)
            {
                StatusCode = errorModel.Status
            };
            return objectResult;
        }
    }
}
