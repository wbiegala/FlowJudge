using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.ReadModels;
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
                workspaceId: dbModel.aggregate_id,
                name: dbModel.name,
                status: Enum.Parse<WorkspaceStatus>(dbModel.status),
                createdAt: dbModel.created_at,
                createdBy: dbModel.created_by,
                members: members);
        }

        public static (WorkspaceDbModel workspace, IEnumerable<WorkspaceMemberDbModel> members) ToDbModel(this WorkspaceRoot domainModel)
        {
            var members = domainModel.Members.Select(member => new WorkspaceMemberDbModel
            {
                id = member.Id,
                workspace_id = domainModel.Id,
                member_id = member.MemberId,
                role = member.Role.ToString(),
                assigned_at = member.AssignedAt,
                assigned_by = member.AssignedBy
            }).ToList();

            var workspace = new WorkspaceDbModel
            {
                id = domainModel.Id,
                aggregate_id = domainModel.AggregateId,
                name = domainModel.Name,
                status = domainModel.Status.ToString(),
                created_at = domainModel.CreatedAt,
                created_by = domainModel.CreatedBy
            };

            return (workspace, members);
        }

        public static WorkspaceListItem ToDomainModel(this WorkspaceListItemDbModel dbModel)
        {
            return new WorkspaceListItem
            {
                Id = dbModel.workspace_id,
                Name = dbModel.name,
                OwnerId = dbModel.owner_id,
                Role = Enum.Parse<WorkspaceRole>(dbModel.role),
                CreatedAt = dbModel.created_at.DateTime
            };
        }
    }
}
