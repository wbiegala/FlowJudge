namespace FlowJudge.API.Service.Auth
{
    public interface IAuthenticationService
    {
        Task<string> GetRegistrationUrlAsync(CancellationToken cancellationToken = default);
    }
}
