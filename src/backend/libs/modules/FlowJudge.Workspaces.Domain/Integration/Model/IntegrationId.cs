using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public sealed record IntegrationId : GuidValueObject
    {
        private IntegrationId(Guid value) : base(value)
        {
        }

        public static IntegrationId Create(Guid value) => new(value);

        public static IntegrationId New() => new(Guid.NewGuid());

        public static bool TryCreate(Guid value, out IntegrationId? integrationId)
        {
            integrationId = null;

            if (value == Guid.Empty)
            {
                return false;
            }

            integrationId = new IntegrationId(value);
            return true;
        }

        public static implicit operator Guid(IntegrationId id) => id.Value;
        public static explicit operator IntegrationId(Guid value) => Create(value);

        public static explicit operator IntegrationId(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                throw new ArgumentException("Integration ID has invalid format.", nameof(value));
            }

            return Create(guid);
        }
    }
}
