using FlowJudge.Common.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.ErrorHandling
{
    public static class ApplicationErrorMapper
    {
        public static ProblemDetails ToProblemDetails(this IError error)
        {
            var problemDetails = new ProblemDetails
            {
                Title = error.Code,
                Type = "https://flowjudge.com/errors/" + error.Code,
                Detail = error.Exception?.ToString(),
                Extensions = error.Properties
            };
            return problemDetails;
        }
    }
}
