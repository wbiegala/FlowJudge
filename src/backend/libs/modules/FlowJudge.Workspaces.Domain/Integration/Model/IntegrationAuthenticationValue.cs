using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public sealed record IntegrationAuthenticationValue : StringValueObject
    {
        public const int MaxLength = 200;

        private IntegrationAuthenticationValue(string value) : base(value)
        {
        }

        public static IntegrationAuthenticationValue Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Integration authentication value cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Integration authentication value cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new IntegrationAuthenticationValue(normalizedValue);
        }

        public static bool TryCreate(string? value, out IntegrationAuthenticationValue? integrationAuthentication)
        {
            integrationAuthentication = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            integrationAuthentication = new IntegrationAuthenticationValue(normalizedValue);
            return true;
        }

        public static implicit operator string(IntegrationAuthenticationValue integrationAuthentication)
            => integrationAuthentication.Value;

        public static explicit operator IntegrationAuthenticationValue(string value)
            => Create(value);
    }
}
