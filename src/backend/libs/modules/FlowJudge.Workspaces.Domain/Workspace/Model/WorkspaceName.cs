using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Workspace.Model
{
    public sealed record WorkspaceName : StringValueObject
    {
        public const int MaxLength = 200;

        private WorkspaceName(string value) : base(value)
        {
        }

        public static WorkspaceName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Workspace name cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Workspace name cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new WorkspaceName(normalizedValue);
        }

        public static bool TryCreate(string? value, out WorkspaceName? workspaceName)
        {
            workspaceName = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            workspaceName = new WorkspaceName(normalizedValue);
            return true;
        }

        public static implicit operator string(WorkspaceName workspaceName)
            => workspaceName.Value;

        public static explicit operator WorkspaceName(string value)
            => Create(value);
    }
}
