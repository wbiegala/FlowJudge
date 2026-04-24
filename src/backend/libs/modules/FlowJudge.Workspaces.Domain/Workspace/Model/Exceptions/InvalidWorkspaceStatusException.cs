using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions
{
    internal class InvalidWorkspaceStatusException : DomainException
    {
        private const string ExceptionErrorCode = "workspace.invalid_status";

        public InvalidWorkspaceStatusException()
            : base(Domain.BoundedContext.Name, nameof(WorkspaceRoot), ExceptionErrorCode)
        {
        }
    }
}
