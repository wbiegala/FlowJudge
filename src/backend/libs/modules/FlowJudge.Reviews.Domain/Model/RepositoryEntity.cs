using FlowJudge.Common.Domain;

namespace FlowJudge.Reviews.Domain.Model
{
    public sealed class RepositoryEntity : Entity
    {
        public Guid SystemId { get; private set; }
        public Guid WorkspaceSystemId { get; private set; }
        public Guid IntegrationSystemId { get; private set; }
        public string ExternalId { get; private set; }
        public int Status { get; private set; }
        public bool IsTrackingEnabled { get; private set; }

        public static class Statuses
        {
            public const int Active = 0;
            public const int Deleted = 1;
        }
    }
}
