using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record ConfigureIntegrationRepositoriesCommand(
        Guid WorkspaceId,
        Guid IntegrationId,
        Guid IssuerId,
        IReadOnlyCollection<RepositoryConfiguration> Repositories) : ICommand;
}
