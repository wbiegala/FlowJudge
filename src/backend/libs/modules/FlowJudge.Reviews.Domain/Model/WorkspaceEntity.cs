using FlowJudge.Common.Domain;

namespace FlowJudge.Reviews.Domain.Model
{
    public sealed class WorkspaceEntity : Entity
    {
        public Guid SystemId { get; private set; }
        public int Status { get; private set; }

        public static class Statuses
        {
            public const int Unactive = 0;
            public const int Active = 1;
            public const int Archived = 2;
        }
    }
}
