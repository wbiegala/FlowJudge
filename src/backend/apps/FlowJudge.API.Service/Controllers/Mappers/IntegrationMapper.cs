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
    }
}
