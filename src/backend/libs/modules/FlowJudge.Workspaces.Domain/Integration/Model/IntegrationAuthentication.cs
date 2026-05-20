using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public sealed class IntegrationAuthentication : Entity
    {
        public Guid IntegrationId { get; private set; }
        public IntegrationAuthenticationType Type { get; private set; }
        public IntegrationAuthenticationStatus Status { get; private set; }
        public IntegrationAuthenticationValue Value { get; private set; }
        public DateTimeOffset? ValidTo { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        internal void Deactivate(DateTimeOffset timestamp)
        {
            Status = IntegrationAuthenticationStatus.Inactive;
            ValidTo = timestamp;
        }

        public static IntegrationAuthentication Create(
            Guid integrationId,
            IntegrationAuthenticationType type,
            IntegrationAuthenticationStatus status,
            IntegrationAuthenticationValue value,
            DateTimeOffset? validTo,
            DateTimeOffset createdAt,
            Guid createdBy)
        {
            return new()
            {
                IntegrationId = integrationId,
                Type = type,
                Status = status,
                Value = value,
                ValidTo = validTo,
                CreatedAt = createdAt,
                CreatedBy = createdBy
            };
        }

        public static IntegrationAuthentication Load(
            Guid id,
            Guid integrationId,
            IntegrationAuthenticationType type,
            IntegrationAuthenticationStatus status,
            string value,
            DateTimeOffset? validTo,
            DateTimeOffset createdAt,
            Guid createdBy)
        {
            return new()
            {
                Id = id,
                IntegrationId = integrationId,
                Type = type,
                Status = status,
                Value = IntegrationAuthenticationValue.Create(value),
                ValidTo = validTo,
                CreatedAt = createdAt,
                CreatedBy = createdBy
            };
        }
    }
}
