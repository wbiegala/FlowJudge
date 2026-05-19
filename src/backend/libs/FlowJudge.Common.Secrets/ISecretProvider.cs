namespace FlowJudge.Common.Secrets
{
    public interface ISecretProvider
    {
        Task<string?> GetSecretAsync(CancellationToken cancellationToken = default);
    }
}
