namespace FlowJudge.Workspaces.Domain
{
    public static class WorkspacesBoundedContext
    {
        public const string Name = "Workspaces";

        public enum Actions
        {
            ViewIntegration,
            CreateIntegration,
            ConnectIntegration,
            DisconnectIntegration,
            ActivateIntegration,
            ConfigureIntegrationRepositories,
        }
    }
}
