namespace FlowJudge.API.Service.Extensions
{
    internal static class WorkspaceContextExtensions
    {
        public const string WorkspaceContextHeaderName = "X-Workspace-Context";

        public static Guid? GetWorkspaceId(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue(WorkspaceContextHeaderName, out var workspaceIdHeader))
            {
                if (Guid.TryParse(workspaceIdHeader, out var workspaceId))
                {
                    return workspaceId;
                }
            }

            return null;
        }
    }
}
