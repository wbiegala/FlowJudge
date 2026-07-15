using FlowJudge.API.Contracts.Repositories;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Domain.Integration.Model;

namespace FlowJudge.API.Service.Controllers.Mappers
{
    public static class RepositoryMapper
    {
        public static GetRepositoriesResponseItem ToListItem(
            this RepositoryListItem repository,
            Func<Guid, IntegrationListItem> getIntegration)
        {
            var integration = getIntegration(repository.IntegrationId);

            return new GetRepositoriesResponseItem
            {
                Id = repository.Id,
                Integration = new IntegrationModel
                {
                    IntegrationId = integration.Id,
                    WorkspaceId = integration.WorkspaceId,
                    Name = integration.Name,
                    Provider = integration.Provider.ToString(),
                    Status = integration.Status.ToString()
                },
                WorkspaceId = integration.WorkspaceId,
                Name = repository.Name,
                FullName = repository.FullName,
                TrackingEnabled = repository.TrackingEnabled,
                Status = repository.Status.ToString()
            };
        }
    }
}
