using FlowJudge.GitHub.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace FlowJudge.GitHub.Client.Auth
{
    internal sealed class InstallationAuthService : IInstallationAuthService
    {
        private readonly bool _isTokenStore;
        private readonly ITokenStore? _tokenStore;
        private readonly bool _isInstallationTokenConfiguration;
        private readonly GitHubClientConfiguration _configuration;
        private readonly InstallationAuthHttpClient _httpClient;

        public InstallationAuthService(IServiceProvider serviceProvider)
        {
            _tokenStore = serviceProvider.GetService<ITokenStore>();
            _isTokenStore = _tokenStore is not null;
            _configuration = serviceProvider.GetRequiredService<GitHubClientConfiguration>();
            _isInstallationTokenConfiguration = _configuration.InstallationTokenAuthConfiguration is not null;
            _httpClient = serviceProvider.GetRequiredService<InstallationAuthHttpClient>();
        }

        public async Task<string> GetInstallationTokenAsync(string installationId, CancellationToken ct = default)
        {
            if (!_isInstallationTokenConfiguration)
                throw new NoConfigurationProvidedException();
            
            var installationToken = await GetStoredInstallationTokenAsync(installationId, ct);
            if (string.IsNullOrWhiteSpace(installationToken))
            {
                installationToken = await ReceiveInstallationTokenAsync(installationId, ct);
            }

            if (string.IsNullOrWhiteSpace(installationToken))
                throw new GitHubClientException($"No installation token for installationId={installationId}.");

            return installationToken;
        }

        private async Task<string?> GetStoredInstallationTokenAsync(string installationId, CancellationToken ct)
        {
            if (!_isTokenStore)
                return null;

            return await _tokenStore!.GetInstallationTokenAsync(installationId, ct);
        }

        private async Task<string?> ReceiveInstallationTokenAsync(string installationId, CancellationToken ct)
        {
            var accessToken = await GetAccessTokenAsync(ct);

            var response = await _httpClient.GetInstallationTokenAsync(installationId, accessToken, ct);

            if (_isTokenStore)
            {
                await _tokenStore!.StoreInstallationTokenAsync(installationId, response.InstallationToken, response.ExpireAt, ct);
            }

            return response.InstallationToken;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            const string access_token_key = "github_access_token";
            string? accessToken = null;
            if (_isTokenStore)
            {
                accessToken = await _tokenStore!.GetInstallationTokenAsync(access_token_key, ct);
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(_configuration.InstallationTokenAuthConfiguration!.PrivateKey);

                var now = DateTimeOffset.UtcNow;

                var securityKey = new RsaSecurityKey(rsa);
                var credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.RsaSha256);

                var expires = now.AddMinutes(9).UtcDateTime;

                var token = new JwtSecurityToken(
                    issuer: _configuration.InstallationTokenAuthConfiguration.ApplicationId,
                    claims: [],
                    notBefore: now.AddSeconds(-60).UtcDateTime,
                    expires: expires,
                    signingCredentials: credentials);

                accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                if (_isTokenStore)
                {
                    await _tokenStore!.StoreInstallationTokenAsync(access_token_key, accessToken, expires, ct);
                }         
            }

            return accessToken;
        }
    }
}
