using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Workspace.Services
{
    public static class WorkspaceRolePermissionsService
    {
        public static bool CanToAction(WorkspaceRole role, WorkspacesBoundedContext.Actions action)
        {
            return role >= _roleRequirements[action];
        }

        private static IReadOnlyDictionary<WorkspacesBoundedContext.Actions, WorkspaceRole> _roleRequirements =
            new Dictionary<WorkspacesBoundedContext.Actions, WorkspaceRole>
            {
                { WorkspacesBoundedContext.Actions.ViewIntegration, WorkspaceRole.Member },
                { WorkspacesBoundedContext.Actions.CreateIntegration, WorkspaceRole.Administrator },
                { WorkspacesBoundedContext.Actions.ConnectIntegration, WorkspaceRole.Administrator },
                { WorkspacesBoundedContext.Actions.DisconnectIntegration, WorkspaceRole.Administrator },
                { WorkspacesBoundedContext.Actions.ActivateIntegration, WorkspaceRole.Administrator },
                { WorkspacesBoundedContext.Actions.ConfigureIntegrationRepositories, WorkspaceRole.Administrator },
            };
    }
}
