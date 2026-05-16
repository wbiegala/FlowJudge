using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public sealed class GithubIntegration : IntegrationRoot
    {
        internal GithubIntegration(): base() { }
        internal GithubIntegration(WorkspaceId workspaceId, IntegrationName name, DateTimeOffset createdAt, Guid createdBy)
            : base(workspaceId, name, IntegrationProvider.GitHub, createdAt, createdBy)
        {

        }

        public static GithubIntegration Load(
            Guid id,
            Guid aggregateId,
            Guid workspaceId,
            string name,
            IntegrationStatus status,
            IEnumerable<IntegrationAuthentication> authenticationData,
            DateTimeOffset createdAt,
            Guid createdBy)
        {
            var integration = new GithubIntegration()
            {
                Id = id,
                AggregateId = IntegrationId.Create(aggregateId),
                WorkspaceId = WorkspaceId.Create(workspaceId),
                Name = IntegrationName.Create(name),
                Provider = IntegrationProvider.GitHub,
                Status = status,
                CreatedAt = createdAt,
                CreatedBy = createdBy
            };

            integration._authenticationData.AddRange(authenticationData);

            return integration;
        }
    }
}
