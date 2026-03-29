using FlowJudge.API.Service.Auth.Common;
using FlowJudge.API.Service.Auth.Exceptions;
using FlowJudge.API.Service.Auth.Keycloak.Client;
using FlowJudge.Common.Cache;
using FlowJudge.Common.Utils.Time;

namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal sealed class KeycloakAuthenticationService(
        IApplicationCache applicationCache,
        ITimeService timeService,
        IKeycloakClient client,
        KeycloakAuthenticationConfiguration configuration)
        : IAuthenticationService
    {
        private const string DefaultResponseType = "code";
        private const string DefaultScope = "openid";

        public Task<string> GetRegistrationUrlAsync(string clientOrigin, CancellationToken cancellationToken = default)
        {
            var url = KeycloakUrlHelper.CreateRegistrationUrl(configuration, clientOrigin);

            return Task.FromResult(url);
        }

        public Task<string> GetRegistrationCallbackUrlAsync(string clientOrigin, CancellationToken cancellationToken = default)
        {
            var url = KeycloakUrlHelper.NormalizeRegistrationConfirmationUrl(configuration, clientOrigin);

            return Task.FromResult(url);
        }

        public async Task<string> InitializeAuthenticationAsync(string uiContext, CancellationToken cancellationToken = default)
        {
            var redirectUrl = KeycloakUrlHelper.CreateCallbackWithRedirectUrl(configuration.LoginCallbackUri, uiContext);

            var state = new AuthenticationState
            {
                Id = Guid.NewGuid(),
                RedirectUri = redirectUrl,
                UiContextUrl = uiContext,
                CreatedAt = timeService.UtcNow,
            };

            await applicationCache.SetObjectAsync(GenerateCacheKey(state.Id), state, TimeSpan.FromMinutes(15), cancellationToken);

            return KeycloakUrlHelper.CreateLoginUrl(
                configuration,
                configuration.ClientId,
                redirectUrl,
                DefaultResponseType,
                DefaultScope,
                state.Id.ToString("N"));
        }

        public async Task ReceiveTokensAsync(
            Guid authenticationStateId,
            string authorizationCode,
            string sessionState,
            string issuer,
            CancellationToken cancellationToken = default)
        {
            var state = await applicationCache.GetObjectAsync<AuthenticationState>(GenerateCacheKey(authenticationStateId), cancellationToken);

            if (state is null)
                throw new AuthenticationStateException(authenticationStateId, "State not found!");

            var tokens = await client.GetTokenAsync(GrantType.AuthorizationCode, authorizationCode, state.RedirectUri, cancellationToken);

            state.AccessToken = tokens.AccessToken;
            state.IdentityToken = tokens.IdentityToken;
            state.RefreshToken = tokens.RefreshToken;

            await applicationCache.SetObjectAsync(GenerateCacheKey(authenticationStateId), state, TimeSpan.FromMinutes(5), cancellationToken);
        }

        public async Task<(string AccessToken, string RefreshToken, string IdToken)> GetTokensAsync(
            Guid authenticationStateId,
            CancellationToken cancellationToken = default)
        {
            var state = await applicationCache.GetObjectAsync<AuthenticationState>(GenerateCacheKey(authenticationStateId), cancellationToken);

            if (state is null)
                throw new AuthenticationStateException(authenticationStateId, "State not found!");

            await applicationCache.RemoveAsync(GenerateCacheKey(authenticationStateId), cancellationToken);

            if (string.IsNullOrWhiteSpace(state.AccessToken) || string.IsNullOrWhiteSpace(state.RefreshToken) || string.IsNullOrWhiteSpace(state.IdentityToken))
                throw new AuthenticationStateException(authenticationStateId, "Tokens not found in state!");

            return new (state.AccessToken, state.RefreshToken, state.IdentityToken);
        }

        public async Task<(string AccessToken, string RefreshToken, string IdToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var tokens = await client.GetTokenAsync(GrantType.RefreshToken, refreshToken, null, cancellationToken);

            return new (tokens.AccessToken, tokens.RefreshToken, tokens.IdentityToken);
        }

        private static string GenerateCacheKey(Guid stateId) =>
            $":authentication-state:{stateId.ToString("N")}";
    }
}
