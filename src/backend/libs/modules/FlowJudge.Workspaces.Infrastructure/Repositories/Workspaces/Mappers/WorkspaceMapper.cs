using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.Mappers
{
    internal static class WorkspaceMapper
    {
        public static WorkspaceRoot ToDomainModel(this WorkspaceDbModel dbModel, IEnumerable<WorkspaceMemberDbModel> memberDbModels)
        {
            var members = new List<WorkspaceMember>();
            foreach (var memberDbModel in memberDbModels)
            {
                var member = WorkspaceMember.Load(
                    id: memberDbModel.id,
                    workspaceId: memberDbModel.workspace_id,
                    memberId: memberDbModel.member_id,
                    role: Enum.Parse<WorkspaceRole>(memberDbModel.role),
                    assignedAt: memberDbModel.assigned_at,
                    assignedBy: memberDbModel.assigned_by);
                members.Add(member);
            }

            return WorkspaceRoot.Load(
                id: dbModel.id,
                workspaceId: dbModel.workspace_id,
                name: dbModel.name,
                status: Enum.Parse<WorkspaceStatus>(dbModel.status),
                createdAt: dbModel.created_at,
                createdBy: dbModel.created_by,
                members: members);
        }
    }
}
