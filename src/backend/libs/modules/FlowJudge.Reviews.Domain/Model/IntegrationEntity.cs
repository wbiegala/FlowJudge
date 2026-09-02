using FlowJudge.Common.Domain;

namespace FlowJudge.Reviews.Domain.Model
{
    public sealed class IntegrationEntity : Entity
    {
        public Guid SystemId { get; private set; }
        public Guid WorkspaceSystemId { get; private set; }
        public int Status { get; private set; }

        public static class Statuses
        {
            public const int Inactive = 0;
            public const int Active = 1;
            public const int Deleted = 2;
        }
    }
}
