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
        public async Task<IActionResult> InitializeRegistrationAsync(CancellationToken cancellationToken = default)
        {
            var registrationUrl = await _authService.GetRegistrationUrlAsync(cancellationToken);

            return Redirect(registrationUrl);
        }
    }
}
