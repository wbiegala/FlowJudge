using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Workspace.Model
{
    public sealed record WorkspaceId : GuidValueObject
    {
        private WorkspaceId(Guid value) : base(value)
        {
        }

        public static WorkspaceId Create(Guid value) => new(value);

        public static WorkspaceId New() => new(Guid.NewGuid());

        public static bool TryCreate(Guid value, out WorkspaceId? workspaceId)
        {
            workspaceId = null;

            if (value == Guid.Empty)
            {
                return false;
            }

            workspaceId = new WorkspaceId(value);
            return true;
        }

        public static implicit operator Guid(WorkspaceId id) => id.Value;
        public static explicit operator WorkspaceId(Guid value) => Create(value);

        public static explicit operator WorkspaceId(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                throw new ArgumentException("Workspace ID has invalid format.", nameof(value));
            }

            return Create(guid);
        }
    }
}