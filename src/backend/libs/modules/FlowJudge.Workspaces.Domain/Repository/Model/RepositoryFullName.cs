using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Repository.Model
{
    public sealed record RepositoryFullName : StringValueObject
    {
        public const int MaxLength = 200;

        private RepositoryFullName(string value) : base(value)
        {
        }

        public static RepositoryFullName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Repository full name cannot be empty.", nameof(value));
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                throw new ArgumentException($"Repository full name cannot be longer than {MaxLength} characters.", nameof(value));
            }

            return new RepositoryFullName(normalizedValue);
        }

        public static bool TryCreate(string? value, out RepositoryFullName? repositoryFullName)
        {
            repositoryFullName = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalizedValue = value.Trim();

            if (normalizedValue.Length > MaxLength)
            {
                return false;
            }

            repositoryFullName = new RepositoryFullName(normalizedValue);
            return true;
        }

        public static implicit operator string(RepositoryFullName repositoryFullName)
            => repositoryFullName.Value;

        public static explicit operator RepositoryFullName(string value)
            => Create(value);
    }
}
