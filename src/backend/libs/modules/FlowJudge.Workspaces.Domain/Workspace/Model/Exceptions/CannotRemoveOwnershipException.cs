using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions
{
    internal class CannotRemoveOwnershipException : DomainException
    {
        private const string ExceptionErrorCode = "workspace.cannot_remove_ownership";

        public CannotRemoveOwnershipException()
            : base(Domain.WorkspacesBoundedContext.Name, nameof(WorkspaceRoot), ExceptionErrorCode)
        {
        }
    }
}
