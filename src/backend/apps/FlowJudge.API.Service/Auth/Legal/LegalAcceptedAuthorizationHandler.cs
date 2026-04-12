using FlowJudge.Common.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace FlowJudge.API.Service.Auth.Legal
{
    public sealed class LegalAcceptedAuthorizationHandler
        : AuthorizationHandler<LegalAcceptedRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILegalLockManager _legalLockManager;

        public LegalAcceptedAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ILegalLockManager legalLockManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _legalLockManager = legalLockManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            LegalAcceptedRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return;
            }

            var endpoint = httpContext.GetEndpoint();

            if (endpoint is null)
            {
                return;
            }

            if (endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            {
                context.Succeed(requirement);
                return;
            }

            if (endpoint.Metadata.GetMetadata<BypassLegalCheckAttribute>() is not null)
            {
                context.Succeed(requirement);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            if (context.User.TryGetUserContext(out var userContext))
            {
                var isLegal = await _legalLockManager.CheckLegalAsync(userContext!.Id, httpContext.RequestAborted);

                if (isLegal)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    httpContext.Items[ErrorCodes.AuthorizationErrorsKey] = ErrorCodes.InvalidLegalStateErrorCode;
                    context.Fail();
                }

                return;
            }

            context.Fail();
        }
    }
}
