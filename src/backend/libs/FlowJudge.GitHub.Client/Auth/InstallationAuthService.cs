using FlowJudge.GitHub.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            var accessToken = GetAccessToken();

            var response = await _httpClient.GetInstallationTokenAsync(installationId, accessToken, ct);

            if (_isTokenStore)
            {
                await _tokenStore!.StoreInstallationTokenAsync(installationId, response.InstallationToken, response.ExpireAt, ct);
            }

            return response.InstallationToken;
        }

        public string GetAccessToken()
        {
            var privateKey = _configuration
                .InstallationTokenAuthConfiguration!
                .PrivateKey
                .Replace("\\n", "\n");

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(9);

            var securityKey = new RsaSecurityKey(rsa)
            {
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                },
                KeyId = _configuration.InstallationTokenAuthConfiguration.ApplicationId
            };

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration.InstallationTokenAuthConfiguration.ApplicationId,
                claims:
                [
                    new Claim(
                JwtRegisteredClaimNames.Iat,
                now.AddSeconds(-60).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
                ],
                expires: expires.UtcDateTime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
