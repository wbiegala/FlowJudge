using FlowJudge.API.Contracts.Integrations;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.API.Service.Controllers.Mappers
{
    public static class IntegrationMapper
    {
        public static GetIntegrationsResponseItem ToResponseItem(
            this IntegrationListItem item,
            Func<Guid, Users.Application.Models.UserData> getCreatorData)
        {
            var creatorData = getCreatorData(item.CreatedBy);

            return new GetIntegrationsResponseItem
            {
                Id = item.Id,
                Name = item.Name,
                Provider = item.Provider.ToString(),
                Status = item.Status.ToString(),
                CreatedAt = item.CreatedAt,
                CreatedBy = new Contracts.Shared.UserData {
                    UserId = creatorData.UserId,
                    UserName = creatorData.UserName,
                    EmailAddress = creatorData.EmailAddress
                }
            };
        }

        public static GetIntegrationDetailsResponse ToResponse(
            this IntegrationData integration,
            Func<Users.Application.Models.UserData> getCreatorData)
        {
            var creatorData = getCreatorData();

            return new GetIntegrationDetailsResponse
            {
                Id = integration.IntegrationId,
                WorkspaceId = integration.WorkspaceId,
                Name = integration.Name,
                Provider = integration.Provider.ToString(),
                Status = integration.Status.ToString(),
                CreatedAt = integration.CreatedAt,
                CreatedBy = new Contracts.Shared.UserData
                {
                    UserId = creatorData.UserId,
                    UserName = creatorData.UserName,
                    EmailAddress = creatorData.EmailAddress
                },
                Repositories = integration.Repositories.Select(repo =>
                    new Contracts.Repositories.GetRepositoriesResponseItem
                    {
                        Id = repo.RepositoryId,
                        Integration = new Contracts.Repositories.IntegrationModel
                        {
                            IntegrationId = integration.IntegrationId,
                            WorkspaceId = integration.WorkspaceId,
                            Name = integration.Name,
                            Provider = integration.Provider.ToString(),
                            Status = integration.Status.ToString()
                        },
                        WorkspaceId = integration.WorkspaceId,
                        Name = integration.Name,
                        FullName = repo.FullName,
                        TrackingEnabled = repo.TrackingEnabled,
                        Status = repo.Status.ToString()
                    })
            };
        }
    }
}
