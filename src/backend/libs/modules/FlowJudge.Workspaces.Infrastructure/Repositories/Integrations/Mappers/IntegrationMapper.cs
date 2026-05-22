using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.Mappers
{
    internal static class IntegrationMapper
    {
        public static IntegrationListItem ToModel(this IntegrationListItemDbModel dbModel)
        {
            return new IntegrationListItem
            {
                Id = dbModel.aggregate_id,
                Name = dbModel.name,
                Provider = Enum.Parse<IntegrationProvider>(dbModel.provider),
                Status = Enum.Parse<IntegrationStatus>(dbModel.status),
                CreatedAt = dbModel.created_at,
                CreatedBy = dbModel.created_by
            };
        }

        public static IntegrationRoot ToDomainModel(this IntegrationDbModel dbModel,
            IEnumerable<IntegrationAuthenticationDbModel> authenticationDbModels)
        {
            var provider = Enum.Parse<IntegrationProvider>(dbModel.provider);

            return provider switch
            {
                IntegrationProvider.GitHub => LoadGithubIntegration(dbModel, authenticationDbModels),
                _ => throw new NotSupportedException($"Integration provider {provider} is not supported.")
            };
        }

        public static (IntegrationDbModel integration, IEnumerable<IntegrationAuthenticationDbModel> authenticationData) ToDbModel(
            this IntegrationRoot integration)
        {
            var authModels = integration.AuthenticationData.Select(ad => new IntegrationAuthenticationDbModel 
            {
                id = ad.Id,
                integration_id = ad.IntegrationId,
                type = ad.Type.ToString(),
                status = ad.Status.ToString(),
                value = ad.Value,
                valid_to = ad.ValidTo,
                created_at = ad.CreatedAt,
                created_by = ad.CreatedBy
            }).ToList();


            var intrgrationModel = new IntegrationDbModel
            {
                id = integration.Id,
                aggregate_id = integration.AggregateId,
                workspace_id = integration.WorkspaceId,
                name = integration.Name,
                provider = integration.Provider.ToString(),
                status = integration.Status.ToString(),
                created_at = integration.CreatedAt,
                created_by = integration.CreatedBy
            };

            return (intrgrationModel, authModels);
        }

        private static GithubIntegration LoadGithubIntegration(IntegrationDbModel dbModel,
            IEnumerable<IntegrationAuthenticationDbModel> authenticationDbModels)
        {
            var authenticationData = authenticationDbModels.Select(ad => IntegrationAuthentication.Load(
                ad.id,
                ad.integration_id,
                Enum.Parse<IntegrationAuthenticationType>(ad.type),
                Enum.Parse<IntegrationAuthenticationStatus>(ad.status),
                ad.value,
                ad.valid_to,
                ad.created_at,
                ad.created_by));

            return GithubIntegration.Load(
                dbModel.id,
                dbModel.aggregate_id,
                dbModel.workspace_id,
                dbModel.name,
                Enum.Parse<IntegrationStatus>(dbModel.status),
                authenticationData,
                dbModel.created_at,
                dbModel.created_by);
        }
    }
}
