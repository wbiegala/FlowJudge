using FlowJudge.API.Contracts;
using FlowJudge.API.Contracts.Auth;
using FlowJudge.API.Service.Auth;
using FlowJudge.API.Service.Auth.Exceptions;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Users.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string RefreshTokenCookieName = "refresh_token";
        private readonly IAuthenticationService _authService;
        private readonly IMediator _mediator;

        public AuthController(IAuthenticationService authService, IMediator mediator)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("register")]
        [AllowAnonymous]
        public async Task<IActionResult> InitializeRegistrationAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string clientOrigin,
            CancellationToken cancellationToken = default)
        {
            //TODO: validate client origin with a whitelist of allowed origins to prevent open redirect vulnerabilities

            var registrationUrl = await _authService.GetRegistrationUrlAsync(clientOrigin, cancellationToken);

            return Redirect(registrationUrl);
        }

        [HttpGet("register-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleRegistrationCallbackAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string clientOrigin,
            CancellationToken cancellationToken = default)
        {
            //TODO: validate client origin with a whitelist of allowed origins to prevent open redirect vulnerabilities
            //TODO: check what keycloak callback sends in this request, maybe something useful xD

            var confirmationUrl = await _authService.GetRegistrationCallbackUrlAsync(clientOrigin, cancellationToken);

            return Redirect(confirmationUrl);
        }


        [HttpGet("login")]
        [AllowAnonymous]
        public async Task<IActionResult> InitializeLoginAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string uiContext,
            CancellationToken cancellationToken = default)
        {
            //TODO: validate client origin with a whitelist of allowed origins to prevent open redirect vulnerabilities

            var loginUrl = await _authService.InitializeAuthenticationAsync(uiContext, cancellationToken);

            return Redirect(loginUrl);
        }

        [HttpGet("login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleLoginCallbackAsync(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string uiContext,
            [FromQuery(Name = "state")] Guid stateId,
            [FromQuery(Name = "session_state")] string sessionState,
            [FromQuery(Name = "iss")] string issuer,
            [FromQuery(Name = "code")] string code,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ReceiveTokensAsync(stateId, code, sessionState, issuer, cancellationToken);

                var redirectUri = new Uri(uiContext);
                var stateParam = new Dictionary<string, string?> { { QueryParams.StateId, stateId.ToString("N") } };
                var fullRedirectUri = QueryHelpers.AddQueryString(redirectUri.ToString(), stateParam);

                return Redirect(fullRedirectUri);
            }
            catch (AuthenticationStateException ex)
            {
                return StatusCode(StatusCodes.Status410Gone, ex.ToProblemDetails(
                    StatusCodes.Status410Gone, ErrorCodes.AuthenticationStateExceptionErrorCode));
            }
            catch (TokenReceiveException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.ToProblemDetails(
                    StatusCodes.Status401Unauthorized, ErrorCodes.TokenReceiveExceptionErrorCode));
            }
        }

        [HttpPost("exchange-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ExchangeTokenAsync(
            [FromQuery] Guid stateId,
            CancellationToken cancellationToken)
        {
            try
            {
                var tokens = await _authService.GetTokensAsync(stateId, cancellationToken);

                var response = new TokenPairResponse
                {
                    AccessToken = tokens.AccessToken,
                    IdentityToken = tokens.IdToken
                };

                AppendRefreshTokenCookie(tokens.RefreshToken);

                return Ok(response);
            }
            catch (AuthenticationStateException ex)
            {
                return StatusCode(StatusCodes.Status410Gone, ex.ToProblemDetails(StatusCodes.Status410Gone, ErrorCodes.AuthenticationStateExceptionErrorCode));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(
            CancellationToken cancellationToken)
        {
            var refreshToken = this.HttpContext.Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return StatusCode(StatusCodes.Status410Gone,
                    ProblemDetailsFactory.CreateProblemDetails(
                        this.HttpContext,
                        statusCode: StatusCodes.Status410Gone,
                        title: ErrorCodes.MissingRefreshTokenErrorCode,
                        detail: "Refresh token cookie is missing"));

            try
            {
                var tokens = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
                var response = new TokenPairResponse
                {
                    AccessToken = tokens.AccessToken,
                    IdentityToken = tokens.IdToken
                };

                AppendRefreshTokenCookie(tokens.RefreshToken);

                return Ok(response);
            }
            catch (TokenReceiveException ex)
            {
                RemoveRefreshTokenCookie();
                return StatusCode(StatusCodes.Status401Unauthorized, ex.ToProblemDetails(
                    StatusCodes.Status401Unauthorized, ErrorCodes.TokenReceiveExceptionErrorCode));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserData(CancellationToken cancellationToken)
        {
            if (this.HttpContext.User.TryGetUserContext(out var userContext))
            {
                var command = new SynchronizeUserCommand { Email = userContext!.Email, UserId = userContext.Id.ToString(), Username = userContext.Username };
                var commandResult = await _mediator.SendCommandAsync(command, cancellationToken);

                var response = new GetUserDataResponse
                {
                    Id = userContext!.Id,
                    Username = userContext!.Username,
                    Email = userContext!.Email,
                };

                return Ok(response);
            }

            return Unauthorized();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync(
            [FromBody] LogoutRequest request,
            CancellationToken cancellationToken)
        {
            var refreshToken = this.HttpContext.Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return StatusCode(StatusCodes.Status410Gone,
                    ProblemDetailsFactory.CreateProblemDetails(
                        this.HttpContext,
                        statusCode: StatusCodes.Status410Gone,
                        title: ErrorCodes.MissingRefreshTokenErrorCode,
                        detail: "Refresh token cookie is missing"));

            var result = await _authService.LogoutAsync(refreshToken, request.IdentityToken, request.UiContext, cancellationToken);

            var response = new LogoutResponse
            {
                LogoutRedirectUrl = result
            };

            return Ok(response);
        }

        [HttpGet("logout-callback")]
        public IActionResult LogoutCallback(
            [FromQuery(Name = AuthQueryParams.UiContextUrlParamName)] string uiContext,
            CancellationToken cancellationToken)
        {
            var uri = new Uri(uiContext);
            var redirectOrigin = uri.GetLeftPart(UriPartial.Authority);
            var reditectUrl = $"{redirectOrigin}/logout";

            RemoveRefreshTokenCookie();

            return Redirect(reditectUrl);
        }

        private void AppendRefreshTokenCookie(string refreshToken)
        {
            this.HttpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
        }

        private void RemoveRefreshTokenCookie()
        {
            this.HttpContext.Response.Cookies.Append(RefreshTokenCookieName, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UnixEpoch,
                MaxAge = TimeSpan.Zero
            });
        }
    }
}
