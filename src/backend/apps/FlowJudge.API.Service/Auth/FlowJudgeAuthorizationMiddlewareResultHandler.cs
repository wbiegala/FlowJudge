using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Auth
{
    internal sealed class FlowJudgeAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private static readonly AuthorizationMiddlewareResultHandler DefaultHandler = new();

        public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
                return;
            }

            if (authorizeResult.Forbidden)
            {
                var code = context.Items.TryGetValue(ErrorCodes.AuthorizationErrorsKey, out var value) && value is string typedValue
                    ? typedValue
                    : ErrorCodes.InsufficientPermissionsErrorCode;

                var problemDetails = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/403",
                    Title = code,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = code switch
                    {
                        ErrorCodes.InvalidLegalStateErrorCode
                            => "User legal state is invalid.",
                        ErrorCodes.InsufficientPermissionsErrorCode
                            => "User does not have sufficient permissions.",
                        _ => "Access to the requested resource is forbidden."
                    }
                };

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problemDetails);
                return;
            }

            await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
