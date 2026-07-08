using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Abstractions.Queries
{
    public sealed record GetIntrgrationDetailsQuery(Guid IntegrationId, Guid WorkspaceId, Guid IssuerId) : IQuery<IntegrationData>;
}
