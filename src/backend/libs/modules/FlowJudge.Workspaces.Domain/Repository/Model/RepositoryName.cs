using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Repository.Model
{
    public sealed record RepositoryName : StringValueObject
    {
        public const int MaxLength = 200;

        private RepositoryName(string value) : base(value)
        {
        }

        public static RepositoryName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Repository name cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Repository name cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new RepositoryName(normalizedValue);
        }

        public static bool TryCreate(string? value, out RepositoryName? repositoryName)
        {
            repositoryName = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            repositoryName = new RepositoryName(normalizedValue);
            return true;
        }

        public static implicit operator string(RepositoryName repositoryName)
            => repositoryName.Value;

        public static explicit operator RepositoryName(string value)
            => Create(value);
    }
}
