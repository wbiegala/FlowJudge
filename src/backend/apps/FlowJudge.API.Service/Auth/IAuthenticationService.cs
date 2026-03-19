namespace FlowJudge.API.Service.Auth
{
    public interface IAuthenticationService
    {
        Task<string> GetRegistrationUrlAsync(string clientOrigin, CancellationToken cancellationToken = default);

        Task<string> GetRegistrationCallbackUrlAsync(string clientOrigin, CancellationToken cancellationToken = default);
    }
}
