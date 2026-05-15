using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Integration.Services
{
    public interface IIntegrationFactory
    {
        /// <summary>
        /// Creates integration for GitHub. The integration will be created with the status of "NotAuthenticated", and the authentication data will be empty.
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace where the integration will be created.</param>
        /// <param name="name">The name of the integration.</param>
        /// <param name="creatorId">The ID of the user creating the integration.</param>
        /// <returns>The created GitHub integration.</returns>
        GithubIntegration CreateGithubIntegration(
            WorkspaceId workspaceId,
            IntegrationName name,
            Guid creatorId);
    }
}
