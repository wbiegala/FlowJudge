using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.Mappers
{
    internal static class RepositoryMapper
    {
        public static RepositoryDbModel ToDbModel(this RepositoryRoot repository)
        {
            return new RepositoryDbModel
            {
                id = repository.Id,
                aggregate_id = repository.AggregateId,
                workspace_id = repository.WorkspaceId,
                integration_id = repository.IntegrationId,
                vcs_external_id = repository.ExternalId,
                name = repository.Name,
                full_name = repository.FullName is null ? null : repository.FullName.Value,
                is_tracking = repository.TrackingEnabled,
                status = repository.Status.ToString(),
            };
        }

        public static RepositoryRoot ToDomain(this RepositoryDbModel model)
        {
            return RepositoryRoot.Load(
                model.id,
                model.aggregate_id,
                model.workspace_id,
                model.integration_id,
                model.vcs_external_id,
                model.name,
                model.full_name,
                model.is_tracking,
                model.status);
        }
    }
}
