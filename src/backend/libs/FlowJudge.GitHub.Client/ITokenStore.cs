namespace FlowJudge.GitHub.Client
{
    public interface ITokenStore
    {
        /// <summary>
        /// Gets the installation token for the given installation Id. This token is used to authenticate GitHub API requests. The token should be cached and refreshed before it expires.
        /// If the token is not available or expired, this method should return null.
        /// </summary>
        /// <param name="installationId">GitHub installation Id</param>
        /// <returns>Installation token if available, otherwise null</returns>
        Task<string?> GetInstallationTokenAsync(string installationId, CancellationToken ct = default);

        /// <summary>
        /// Stores the installation token for the given installation Id. This method is called by the GitHub client after it retrieves a new token from GitHub API.
        /// The implementation should store the token along with its expiration time and return it when requested by GetInstallationTokenAsync.
        /// </summary>
        /// <param name="installationId">GitHub installation Id</param>
        /// <param name="token">Installation token</param>
        /// <param name="expirationTime">The expiration time of the token</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task StoreInstallationTokenAsync(string installationId, string token, DateTimeOffset expirationTime, CancellationToken ct = default);
    }
}
