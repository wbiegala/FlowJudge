using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.Users.Domain.Model
{
    public sealed record UserIdentityId : GuidValueObject
    {
        private UserIdentityId(Guid value) : base(value)
        {
        }

        public static UserIdentityId Create(Guid value) => new(value);

        public static UserIdentityId New() => new(Guid.NewGuid());

        public static bool TryCreate(Guid value, out UserIdentityId? userId)
        {
            userId = null;

            if (value == Guid.Empty)
            {
                return false;
            }

            userId = new UserIdentityId(value);
            return true;
        }

        public static implicit operator Guid(UserIdentityId id) => id.Value;

        public static explicit operator UserIdentityId(Guid value) => Create(value);

        public static explicit operator UserIdentityId(string value)
        {
            if (!Guid.TryParse(value, out var guid))
            {
                throw new ArgumentException("User identity ID has invalid format.", nameof(value));
            }

            return Create(guid);
        }
    }
}
