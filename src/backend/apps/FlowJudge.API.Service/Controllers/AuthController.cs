using FlowJudge.API.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("register")]
        [AllowAnonymous]
        public async Task<IActionResult> InitializeRegistrationAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)]string clientOrigin,
            CancellationToken cancellationToken = default)
        {
            //TODO: validate client origin with a whitelist of allowed origins to prevent open redirect vulnerabilities

            var registrationUrl = await _authService.GetRegistrationUrlAsync(clientOrigin, cancellationToken);

            return Redirect(registrationUrl);
        }

        [HttpGet("register-callback")]
        public async Task<IActionResult> HandleRegistrationCallbackAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string clientOrigin,
            CancellationToken cancellationToken = default)
        {
            //TODO: validate client origin with a whitelist of allowed origins to prevent open redirect vulnerabilities
            //TODO: check what keycloak callback sends in this request, maybe something useful xD

            var confirmationUrl = await _authService.GetRegistrationCallbackUrlAsync(clientOrigin, cancellationToken);

            return Redirect(confirmationUrl);
        }
    }
}
