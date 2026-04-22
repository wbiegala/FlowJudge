using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions
{
    internal sealed class ForbiddenActionException : DomainException
    {
        private const string ExceptionErrorCode = "workspace.insufficient_permissions";

        public ForbiddenActionException()
            : base(Domain.BoundedContext.Name, nameof(WorkspaceRoot), ExceptionErrorCode)
        {
        }
    }
}
