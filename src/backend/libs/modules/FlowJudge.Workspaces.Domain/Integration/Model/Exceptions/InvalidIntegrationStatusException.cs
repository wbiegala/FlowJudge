using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Integration.Model.Exceptions
{
    public class InvalidIntegrationStatusException : DomainException
    {
        private const string ExceptionErrorCode = "integration.invalid_status";

        public InvalidIntegrationStatusException()
            : base(Domain.WorkspacesBoundedContext.Name, nameof(IntegrationRoot), ExceptionErrorCode)
        {
        }
    }
}
