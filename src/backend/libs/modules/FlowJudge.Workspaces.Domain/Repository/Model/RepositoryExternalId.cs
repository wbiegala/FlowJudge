using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Repository.Model
{
    public sealed record RepositoryExternalId : StringValueObject
    {
        public const int MaxLength = 200;

        private RepositoryExternalId(string value) : base(value)
        {
        }

        public static RepositoryExternalId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Repository external ID cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Repository external ID cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new RepositoryExternalId(normalizedValue);
        }

        public static bool TryCreate(string? value, out RepositoryExternalId? repositoryExternalId)
        {
            repositoryExternalId = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            repositoryExternalId = new RepositoryExternalId(normalizedValue);
            return true;
        }

        public static implicit operator string(RepositoryExternalId repositoryExternalId)
            => repositoryExternalId.Value;

        public static explicit operator RepositoryExternalId(string value)
            => Create(value);
    }
}
