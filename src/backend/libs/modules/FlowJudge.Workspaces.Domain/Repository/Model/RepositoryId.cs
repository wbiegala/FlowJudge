using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Workspaces.Domain.Repository.Model
{
    public sealed record RepositoryId : GuidValueObject
    {
        private RepositoryId(Guid value) : base(value)
        {
        }

        public static RepositoryId Create(Guid value) => new(value);

        public static RepositoryId New() => new(Guid.NewGuid());

        public static bool TryCreate(Guid value, out RepositoryId? repositoryId)
        {
            repositoryId = null;

            if (value == Guid.Empty)
            {
                return false;
            }

            repositoryId = new RepositoryId(value);
            return true;
        }

        public static implicit operator Guid(RepositoryId id) => id.Value;
        public static explicit operator RepositoryId(Guid value) => Create(value);

        public static explicit operator RepositoryId(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                throw new ArgumentException("Repository ID has invalid format.", nameof(value));
            }

            return Create(guid);
        }
    }
}
