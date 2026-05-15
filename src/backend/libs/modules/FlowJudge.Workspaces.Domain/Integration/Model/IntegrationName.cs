using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public sealed record IntegrationName : StringValueObject
    {
        public const int MaxLength = 200;

        private IntegrationName(string value) : base(value)
        {
        }

        public static IntegrationName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Integration name cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Integration name cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new IntegrationName(normalizedValue);
        }

        public static bool TryCreate(string? value, out IntegrationName? integrationName)
        {
            integrationName = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            integrationName = new IntegrationName(normalizedValue);
            return true;
        }

        public static implicit operator string(IntegrationName integrationName)
            => integrationName.Value;

        public static explicit operator IntegrationName(string value)
            => Create(value);
    }
}
