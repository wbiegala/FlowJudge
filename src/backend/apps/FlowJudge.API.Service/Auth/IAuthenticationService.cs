namespace FlowJudge.API.Service.Auth
{
    public interface IAuthenticationService
    {
        Task<string> GetRegistrationUrlAsync(string clientOrigin, CancellationToken cancellationToken = default);

        Task<string> GetRegistrationCallbackUrlAsync(string clientOrigin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the authentication process by generating the appropriate login URL based on the provided UI context (e.g., client origin).
        /// </summary>
        /// <param name="uiContext">Current UI url</param>
        /// <returns>URL to login page</returns>
        Task<string> InitializeAuthenticationAsync(string uiContext, CancellationToken cancellationToken = default);

        Task ReceiveTokensAsync(
            Guid authenticationStateId,
            string authorizationCode,
            string sessionState,
            string issuer,
            CancellationToken cancellationToken = default);

        Task<(string AccessToken, string RefreshToken, string IdToken)> GetTokensAsync(Guid authenticationStateId, CancellationToken cancellationToken = default);

        Task<(string AccessToken, string RefreshToken, string IdToken)> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        Task<string> LogoutAsync(string refreshToken, string identityToken, string uiContext, CancellationToken cancellationToken = default);
    }
}
