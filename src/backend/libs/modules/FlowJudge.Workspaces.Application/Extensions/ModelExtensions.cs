using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Extensions
{
    public static class ModelExtensions
    {
        public static WorkspaceData ToAbstraction(this Domain.Workspace.Model.WorkspaceRoot workspace)
        {
            return new WorkspaceData
            {
                WorkspaceId = workspace.AggregateId,
                Name = workspace.Name.Value,
                Status = workspace.Status,
                CreatedAt = workspace.CreatedAt,
                CreatedBy = workspace.CreatedBy,
                Members = workspace.Members.Select(m => new WorkspaceData.WorkspaceMemberData
                {
                    MemberId = m.MemberId,
                    Role = m.Role,
                    AssingedBy = m.AssignedBy,
                    AssignedAt = m.AssignedAt
                }).ToList()
            };
        }
    }
}
