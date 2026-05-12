using FlowJudge.API.Contracts.Workspaces;
using FlowJudge.Users.Application.Models;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.API.Service.Controllers.Mappers
{
    public static class WorkspaceMapper
    {
        public static GetWorkspacesResponseItem ToResponseItem(this WorkspaceListItem item, Func<Guid, UserData> getOwnerData)
        {
            var owner = getOwnerData(item.OwnerId);

            return new GetWorkspacesResponseItem
            {
                WorkspaceId = item.Id,
                Name = item.Name,
                Owner = new WorkspaceUserData
                { 
                    UserId = owner.UserId,
                    UserName = owner.UserName,
                    EmailAddress = owner.EmailAddress,
                },
                RoleName = item.Role.ToString(),
                CreatedAt = item.CreatedAt
            };
        }

        public static GetWorkspaceResponse ToResponse(this WorkspaceData workspace, Func<Guid, UserData> getUserData)
        {
            var owner = getUserData(workspace.CreatedBy);

            return new GetWorkspaceResponse
            {
                Id = workspace.WorkspaceId,
                Name = workspace.Name,
                Status = workspace.Status.ToString(),
                CreatedAt = workspace.CreatedAt,
                CreatedBy = new WorkspaceUserData
                {
                    UserId = owner.UserId,
                    UserName = owner.UserName,
                    EmailAddress = owner.EmailAddress,
                },
                Members = workspace.Members.Select(member =>
                {
                    var memberData = getUserData(member.MemberId);
                    var assinerData = member.AssingedBy.HasValue ? getUserData(member.AssingedBy.Value) : null;

                    return new WorkspaceMemberData
                    {
                        Member = new WorkspaceUserData                         {
                            UserId = memberData.UserId,
                            UserName = memberData.UserName,
                            EmailAddress = memberData.EmailAddress,
                        },
                        Role = member.Role.ToString(),
                        AssingedAt = member.AssignedAt,
                        AssignedBy = assinerData is not null
                        ? new WorkspaceUserData
                            {
                                UserId = assinerData.UserId,
                                UserName = assinerData.UserName,
                                EmailAddress = assinerData.EmailAddress,
                            }
                        : null
                    };
                })
            };
        }

        internal static IEnumerable<Guid> GetRelatedUsersIds(this WorkspaceData workspace)
        {
            var membersIds = workspace.Members.Select(member => member.MemberId);
            var assignersIds = workspace.Members.Where(m => m.AssingedBy.HasValue).Select(m => m.AssingedBy!.Value);
            var relatedUsersIds = membersIds.Concat(assignersIds).Append(workspace.CreatedBy).Distinct();

            return relatedUsersIds.ToList();
        }
    }
}
