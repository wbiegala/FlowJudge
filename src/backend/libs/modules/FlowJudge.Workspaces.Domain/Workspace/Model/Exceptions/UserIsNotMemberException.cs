using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions
{
    internal class UserIsNotMemberException : DomainException
    {
        private const string ExceptionErrorCode = "workspace.user_is_not_member";

        public UserIsNotMemberException()
            : base(Domain.WorkspacesBoundedContext.Name, nameof(WorkspaceRoot), ExceptionErrorCode)
        {
        }
    }
}
