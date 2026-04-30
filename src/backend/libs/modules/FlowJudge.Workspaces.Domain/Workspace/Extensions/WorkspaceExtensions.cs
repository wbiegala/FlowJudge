using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Workspace.Extensions
{
    public static class WorkspaceExtensions
    {
        public static bool IsUserMember(this WorkspaceRoot aggregate, Guid userId) =>
            aggregate.Members.Any(m => m.MemberId == userId);
    }
}
